using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using RedLine.Extensions.Hosting;
using Serilog.Core;

namespace Zinc.DeveloperCenter.IntegrationTests.Web
{
    internal static class Startup
    {
        public static IHostBuilder CreateHostBuilder(params ILogEventSink[] additionalLogSinks)
        {
            Environment.SetEnvironmentVariable("APP_ENTRYPOINT", typeof(Startup).Namespace);
            var hostName = typeof(Startup).Namespace;
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(null)
                .ConfigureAppConfiguration(builder => builder.AddParameterStoreSettings(null, "test"))
                .ConfigureApplicationContext()
                .ConfigureSerilog(hostName, additionalLogSinks)
                .ConfigureNServiceBus(hostName, EndpointType.SendOnly)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                        options.AllowSynchronousIO = true;
                    });
                    webBuilder.UseStartup<Host.Web.Startup>();
                    webBuilder.ConfigureTestServices(services =>
                    {
                        /*
                        * Need to replace some services with mocks or stubs? Here's the place to do it.
                        */
                    });
                });
        }
    }
}
