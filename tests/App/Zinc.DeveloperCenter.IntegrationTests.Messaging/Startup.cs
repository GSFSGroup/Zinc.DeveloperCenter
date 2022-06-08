using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedLine.Domain;
using RedLine.Extensions.Hosting;
using Serilog.Core;

namespace Zinc.DeveloperCenter.IntegrationTests.Messaging
{
    internal static class Startup
    {
        public static IHostBuilder CreateHostBuilder(params ILogEventSink[] additionalLogSinks)
        {
            Environment.SetEnvironmentVariable("APP_ENTRYPOINT", typeof(Startup).Namespace);
            var hostName = typeof(Startup).Namespace;
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null)
                .ConfigureAppConfiguration(builder => builder.AddParameterStoreSettings(Environment.GetEnvironmentVariable("RL_APP_NAME"), "test"))
                .ConfigureApplicationContext()
                .ConfigureSerilog(hostName, additionalLogSinks)
                .ConfigureNServiceBus(hostName, EndpointType.FullDuplex)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.AllowSynchronousIO = true;
                    });
                    webBuilder.UseStartup<Host.Messaging.Startup>();
                    webBuilder.ConfigureTestServices(services =>
                    {
                        services.AddTransient<ITenantId>(_ => new TenantId(WellKnownIds.TenantId));
                        /*
                        * Need to replace some services with mocks or stubs? Here's the place to do it.
                        */
                    });
                });
        }
    }
}
