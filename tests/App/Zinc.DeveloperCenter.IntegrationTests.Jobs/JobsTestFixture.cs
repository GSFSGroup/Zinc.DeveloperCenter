using System;
using Alba;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RedLine.Data;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Data.Migrations;

namespace Zinc.DeveloperCenter.IntegrationTests.Jobs
{
    public class JobsTestFixture : IDisposable
    {
        private readonly TestOutputSink sink = new TestOutputSink();

        /// <summary>
        /// This constructor initializes tests for running web scenarios with a database.
        /// </summary>
        public JobsTestFixture()
        {
            var services = Startup
                .CreateHostBuilder(sink)
                .ConfigureServices((context, container) => container.AddFluentMigrator(context.Configuration))
                .Build()
                .Services;
            var scopeFactory = services.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<Migrator>().RunMigrations();
            }

            TestHost = new AlbaHost(Startup.CreateHostBuilder(sink));
            ConnectionString = services.GetRequiredService<PostgresConnectionString>();
        }

        public IAlbaHost TestHost { get; }

        public PostgresConnectionString ConnectionString { get; }

        public void RegisterTestOutputHelper(ITestOutputHelper output)
        {
            sink.Register(output);
        }

        public void UnregisterTestOutputHelper()
        {
            sink.Unregister();
        }

        /// <summary>
        /// This method destroys the database created in the constructor after all tests have run.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Destroy the database...
                var databaseName = ConnectionString.DatabaseName;

                using (var connection = new NpgsqlConnection(ConnectionString.ServerConnectionString))
                {
                    connection.Execute($@"
                        SELECT pg_terminate_backend(pg_stat_activity.pid)
                        FROM pg_stat_activity
                        WHERE pg_stat_activity.datname = '{databaseName}';");

                    connection.Execute($"DROP DATABASE \"{databaseName}\";");
                }

                TestHost?.Dispose();
            }
        }
    }
}

