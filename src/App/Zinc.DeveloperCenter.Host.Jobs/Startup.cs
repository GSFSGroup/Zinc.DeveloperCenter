using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RedLine.Extensions.Hosting;
using RedLine.Extensions.Hosting.Jobs;
using Zinc.DeveloperCenter.Application;
using Zinc.DeveloperCenter.Data;
using Zinc.DeveloperCenter.Host.Jobs.Outbox;
using Zinc.DeveloperCenter.Host.Jobs.RefreshAdrs;

namespace Zinc.DeveloperCenter.Host.Jobs
{
    internal class Startup
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="configuration">The application configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRedLineJobHost(WithJobs)
                .AddRedLineHealthChecks(Configuration, WithCustomHealthChecks)
                .AddDataServices()
                .AddApplicationServices(Configuration)
                ;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseRedLineJobHost();
        }

        private void WithJobs(IServiceCollectionQuartzConfigurator quartz)
        {
            OutboxJob.ConfigureJob(quartz, Configuration);
            RefreshGsfsGroupAdrsJob.ConfigureJob(quartz, Configuration);
            /* Add future jobs here */
        }

        private void WithCustomHealthChecks(IHealthChecksBuilder healthChecks)
        {
            OutboxJob.ConfigureHealthCheck(healthChecks, Configuration);
            RefreshGsfsGroupAdrsJob.ConfigureHealthCheck(healthChecks, Configuration);
            /* Add future job health checks here */
        }
    }
}
