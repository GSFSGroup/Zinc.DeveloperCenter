using System;
using System.Net.Http.Headers;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLine.Application;
using Zinc.DeveloperCenter.Application.Services.Favorites;
using Zinc.DeveloperCenter.Application.Services.GitHub;
using Zinc.DeveloperCenter.Application.Services.MostViewed;
using Zinc.DeveloperCenter.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Services.Favorites;
using Zinc.DeveloperCenter.Domain.Services.GitHub;
using Zinc.DeveloperCenter.Domain.Services.MostViewed;

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
                .AddScoped<IMostViewedService, MostViewedService>()
                .AddScoped<IFavoritesService, FavoritesService>()
                .AddScoped(_ => configuration.GetSection(GitHubApiConfig.SectionName).Get<GitHubApiConfig>())
                .AddHttpClient<IGitHubApiService, GitHubApiService>(client =>
                {
                    client.BaseAddress = new Uri(GitHubApiBaseAddress);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RedLine", "1.0"));
                });

            return services;
        }
    }
}
