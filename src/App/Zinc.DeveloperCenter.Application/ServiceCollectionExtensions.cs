using System;
using System.Net.Http.Headers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLine.Application;
using Zinc.DeveloperCenter.Application.Services;
using Zinc.DeveloperCenter.Application.Services.Favorites;
using Zinc.DeveloperCenter.Application.Services.GitHub;
using Zinc.DeveloperCenter.Application.Services.ViewCounter;
using Zinc.DeveloperCenter.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Services.Favorites;
using Zinc.DeveloperCenter.Domain.Services.GitHub;
using Zinc.DeveloperCenter.Domain.Services.ViewCounter;

namespace Zinc.DeveloperCenter.Application
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> used to register dependencies with the IoC container.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        private static readonly string GitHubApiBaseAddress = "https://api.github.com";

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
                .AddScoped<IViewCounterService, ViewCounterService>()
                .AddScoped<IFavoritesService, FavoritesService>()
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
            services
                .AddScoped(_ => configuration.GetSection(GitHubApiConfig.SectionName).Get<GitHubApiConfig>())
                .AddHttpClient<IGitHubApiService, GitHubApiService>(client =>
                {
                    client.BaseAddress = new Uri(GitHubApiBaseAddress);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RedLine", "1.0"));
                });

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
