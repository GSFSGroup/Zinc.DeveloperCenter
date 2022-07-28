using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using RedLine.Extensions.Hosting;
using RedLine.Extensions.Hosting.Jobs;
using System.Linq;
using System.Reflection;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "By design.")]
        private void WithJobs(IServiceCollectionQuartzConfigurator quartz)
        {
            var jobs = typeof(AssemblyMarker).Assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericTypeDefinition && x.IsAssignableTo(typeof(IJob)))
                .ToList();

            foreach (var job in jobs)
            {
                var configureMethod = job.GetMethod("ConfigureJob", BindingFlags.NonPublic | BindingFlags.Static);

                if (configureMethod == null)
                {
                    throw new System.InvalidOperationException($"The job '{job.Name}' does not have a static ConfigureJob() method.");
                }

                configureMethod!.Invoke(job, new object[] { quartz, Configuration });
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "By design.")]
        private void WithCustomHealthChecks(IHealthChecksBuilder healthChecks)
        {
            var jobs = typeof(AssemblyMarker).Assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericTypeDefinition && x.IsAssignableTo(typeof(IJob)))
                .ToList();

            foreach (var job in jobs)
            {
                var configureMethod = job.GetMethod("ConfigureHealthCheck", BindingFlags.NonPublic | BindingFlags.Static);

                if (configureMethod == null)
                {
                    throw new System.InvalidOperationException($"The job '{job.Name}' does not have a static ConfigureHealthCheck() method.");
                }

                configureMethod!.Invoke(job, new object[] { healthChecks });
            }

            // OutboxJob.ConfigureHealthCheck(healthChecks, Configuration)
            // RefreshGsfsGroupAdrsJob.ConfigureHealthCheck(healthChecks, Configuration)
            // RefreshGsfsGroupAdrsLastUpdatedJob.ConfigureHealthCheck(healthChecks, Configuration)
            /* Add future job health checks here */
        }
    }
}
