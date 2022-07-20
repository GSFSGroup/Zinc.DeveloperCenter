using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLine.Application;
using Zinc.DeveloperCenter.Application.Services;
using Zinc.DeveloperCenter.Application.Services.GitHub;
using Zinc.DeveloperCenter.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Services.GitHub;

namespace Zinc.DeveloperCenter.Application
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> used to register dependencies with the IoC container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the application services to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <param name="configuration">The GitHub Service Configuration.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMediatR(typeof(AssemblyMarker))
                .AddFluentValidation<AssemblyMarker>()
                .AddAutoMapper(typeof(AssemblyMarker))
                .AddActivities<AssemblyMarker>()
                .AddApiServices(configuration)
                .AddScoped<IApplicationRepository, ApplicationRepository>()
                .AddScoped<IArchitectureDecisionRecordRepository, ArchitectureDecisionRecordRepository>()
                .AddScoped<IGitHubApiService, GitHubApiService>()
                ;
            return services;
        }

        /// <summary>
        /// Adds the api services to the container.
        /// </summary>
        /// <param name="services">The IoC container.</param>
        /// <param name="configuration">The GitHub Service Configuration.</param>
        /// <returns><see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // GitHub
            var gitHubServiceConfig = configuration
                .GetSection(GitHubServiceConfig.SectionName)
                .Get<GitHubServiceConfig>();

            if (gitHubServiceConfig.Enabled)
            {
                services
                    .AddHttpClient<IGitHubService, GitHubService>();
            }
            else
            {
                services.AddTransient<IGitHubService, FakeGitHubService>();
            }

            return services;
        }
    }
}
