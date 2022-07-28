using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Zinc.DeveloperCenter.Application.Exceptions;
using Zinc.DeveloperCenter.Domain;
using Zinc.DeveloperCenter.Domain.Services.GitHub;

namespace Zinc.DeveloperCenter.Application.Services.GitHub
{
    /// <summary>
    /// Provides an implementation of the <see cref="IGitHubApiService"/>.
    /// </summary>
    public class GitHubApiService : IGitHubApiService
    {
        private readonly GitHubApiConfig config;
        private readonly HttpClient httpClient;
        private readonly ILogger<GitHubApiService> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="config">The service configuration settings.</param>
        /// <param name="httpClient">The <see cref="HttpClient"/> used to interact with the GitHub api.</param>
        /// <param name="logger">A diagnostic logger.</param>
        public GitHubApiService(
            GitHubApiConfig config,
            HttpClient httpClient,
            ILogger<GitHubApiService> logger)
        {
            this.config = config;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4457:Parameter validation in \"async\"/\"await\" methods should be wrapped", Justification = "Go F_ yourself.")]
        public async Task<(string? Content, string? ContentUrl)> DownloadArchitectureDecisionRecord(
            string tenantId,
            string repositoryName,
            string filePath,
            FileFormat fileFormat)
        {
            var tenantConfig = config.Tenants.FirstOrDefault(x => x.TenantId == tenantId);

            if (tenantConfig == null)
            {
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]");
            }

            if (tenantConfig.Disabled)
            {
                logger.LogWarning("The {Service} for tenant {TenantId} is disabled. The request will not be processed.", GetType().Name, tenantId);
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]", "Disabled");
            }

            if (fileFormat == FileFormat.Unknown)
            {
                throw new ArgumentException($"The '{nameof(fileFormat)}' argument is invalid.", nameof(fileFormat));
            }

            var orgName = string.IsNullOrEmpty(tenantConfig.OrgName)
                ? tenantConfig.TenantId
                : tenantConfig.OrgName;

            var endpoint = $"repos/{orgName}/{repositoryName}/contents/{filePath}";

            var content = await ServiceCaller.MakeCall(httpClient, endpoint, tenantConfig.AccessToken, fileFormat.ToDescription()).ConfigureAwait(false) ?? string.Empty;
            var contentUrl = $"https://github.com/{orgName}/{repositoryName}/blob/HEAD/{filePath}";

            if (content!.Length == 0)
            {
                return default;
            }

            return (Content: content, ContentUrl: contentUrl);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> FindArchitectureDecisionRecords(string tenantId, int page, int pageSize)
        {
            var tenantConfig = config.Tenants.FirstOrDefault(x => x.TenantId == tenantId);

            if (tenantConfig == null)
            {
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]");
            }

            if (tenantConfig.Disabled)
            {
                logger.LogWarning("The {Service} for tenant {TenantId} is disabled. The request will not be processed.", GetType().Name, tenantId);
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]", "Disabled");
            }

            return await FindArchitectureDecisionRecords(
                tenantConfig,
                string.Empty,
                page,
                pageSize).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> FindArchitectureDecisionRecords(string tenantId, string repositoryName)
        {
            var tenantConfig = config.Tenants.FirstOrDefault(x => x.TenantId == tenantId);

            if (tenantConfig == null)
            {
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]");
            }

            if (tenantConfig.Disabled)
            {
                logger.LogWarning("The {Service} for tenant {TenantId} is disabled. The request will not be processed.", GetType().Name, tenantId);
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]", "Disabled");
            }

            return await FindArchitectureDecisionRecords(
                tenantConfig,
                repositoryName,
                1,
                100).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<(string? LastUpdatedBy, DateTimeOffset? LastUpdatedOn)> GetLastUpdatedDetails(string tenantId, string repositoryName, string filePath)
        {
            var tenantConfig = config.Tenants.FirstOrDefault(x => x.TenantId == tenantId);

            if (tenantConfig == null)
            {
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]");
            }

            if (tenantConfig.Disabled)
            {
                logger.LogWarning("The {Service} for tenant {TenantId} is disabled. The request will not be processed.", GetType().Name, tenantId);
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]", "Disabled");
            }

            return await GetLastUpdatedDetails(tenantConfig, repositoryName, filePath).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GitHubRepositoryModel>> GetRepositories(string tenantId)
        {
            var tenantConfig = config.Tenants.FirstOrDefault(x => x.TenantId == tenantId);

            if (tenantConfig == null)
            {
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]");
            }

            if (tenantConfig.Disabled)
            {
                logger.LogWarning("The {Service} for tenant {TenantId} is disabled. The request will not be processed.", GetType().Name, tenantId);
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApi:Tenants[{tenantId}]", "Disabled");
            }

            var results = new HashSet<GitHubRepositoryModel>(256);

            var page = 1;
            var pageSize = 100; // 100 is the max

            var repos = await GetRepositories(tenantConfig, page, pageSize).ConfigureAwait(false);

            while (repos.Count > 0)
            {
                results.UnionWith(repos);

                page++;

                if (repos.Count == pageSize)
                {
                    // GitHub doesn't like rapid-fire requests
                    await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
                }
                else if (repos.Count < pageSize)
                {
                    break;
                }

                repos = await GetRepositories(tenantConfig, page, pageSize).ConfigureAwait(false);
            }

            if (results.Count == 0)
            {
                logger.LogWarning("Failed to find any repositories for {Org}.", tenantConfig.OrgName ?? tenantConfig.TenantId);
            }

            return results;
        }

        private async Task<HashSet<GitHubArchitectureDecisionRecordModel>> FindArchitectureDecisionRecords(
            GitHubApiConfig.TenantConfig tenantConfig,
            string repositoryName,
            int page,
            int pageSize)
        {
            var orgName = string.IsNullOrEmpty(tenantConfig.OrgName)
                ? tenantConfig.TenantId
                : tenantConfig.OrgName;

            // NOTE: This url assumes there will never be more than 100 ADRs in a given app
            var endpoint = string.IsNullOrEmpty(repositoryName)
                ? $"search/code?q=adr+in:path+language:markdown+org:{orgName}&page={page}&per_page={pageSize}"
                : $"search/code?q=adr+in:path+language:markdown+org:{orgName}+repo:{orgName}/{repositoryName}&page={page}&per_page={pageSize}";

            var results = new HashSet<GitHubArchitectureDecisionRecordModel>(100);

            var model = await ServiceCaller.MakeCall<FileSearchResultModel>(httpClient, endpoint, tenantConfig.AccessToken)
                 .ConfigureAwait(false)
                 ?? new FileSearchResultModel();

            // The check for .md or .markdown seems to be unnecessary, but just in case
            var items = model.items?
                .Where(x => x.path!.EndsWith(".md") || x.path!.EndsWith(".markdown"))
                .ToList() ?? new List<FileSearchResultModel.ItemModel>();

            foreach (var item in items)
            {
                results.Add(new GitHubArchitectureDecisionRecordModel(
                    tenantConfig.TenantId,
                    item.repository?.name!,
                    item.path!));
            }

            return results;
        }

        private async Task<(string? LastUpdatedBy, DateTimeOffset? LastUpdatedOn)> GetLastUpdatedDetails(
            GitHubApiConfig.TenantConfig tenantConfig,
            string repositoryName,
            string filePath)
        {
            var orgName = string.IsNullOrEmpty(tenantConfig.OrgName)
                ? tenantConfig.TenantId
                : tenantConfig.OrgName;

            var endpoint = $"repos/{orgName}/{repositoryName}/commits?path={filePath}&page=1&per_page=1&sort=committer-date&order=desc";

            var result = (await ServiceCaller.MakeCall<List<CommitSearchModel>>(httpClient, endpoint, tenantConfig.AccessToken)
                .ConfigureAwait(false)
                ?? new List<CommitSearchModel>())
                .FirstOrDefault();

            if (result == null || result.commit == null || result.commit.committer == null)
            {
                logger.LogWarning("Failed to get commit details for ADR {ADR}.", filePath);
                return default;
            }

            return (LastUpdatedBy: result.commit.committer.name, LastUpdatedOn: result.commit.committer.date);
        }

        private async Task<List<GitHubRepositoryModel>> GetRepositories(
            GitHubApiConfig.TenantConfig tenantConfig,
            int page,
            int pageSize)
        {
            if (string.IsNullOrWhiteSpace(tenantConfig.AccessToken))
            {
                throw new RedLine.Domain.Exceptions.InvalidConfigurationException($"GitHubApiConfig:Tenants[{tenantConfig.TenantId!}]:AccessToken");
            }

            var orgName = string.IsNullOrEmpty(tenantConfig.OrgName)
                ? tenantConfig.TenantId
                : tenantConfig.OrgName;

            var endpoint = $"orgs/{orgName!}/repos?page={page}&per_page={pageSize}";

            var model = await ServiceCaller.MakeCall<List<RepositorySearchModel>>(httpClient, endpoint, tenantConfig.AccessToken)
                .ConfigureAwait(false)
                ?? new List<RepositorySearchModel>();

            return model
                .Select(model => new GitHubRepositoryModel(tenantConfig.TenantId!, model.name!, model.html_url!, model.description))
                .ToList();
        }

        private static class ServiceCaller
        {
            public static async Task<string?> MakeCall(HttpClient httpClient, string endpoint, string accessToken, params string[] acceptHeaders)
            {
                var totalRetries = 0;

                while (totalRetries < 3)
                {
                    using var request = CreateMessage(endpoint, accessToken, acceptHeaders);
                    using (var response = await httpClient.SendAsync(request).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            var error = JsonConvert.DeserializeAnonymousType(
                                await response.Content.ReadAsStringAsync().ConfigureAwait(false) ?? "{}",
                                new { message = (string?)null });

                            if (IsRecoverableError((response: response, message: error?.message), out var waitTime))
                            {
                                await Task.Delay(waitTime).ConfigureAwait(false);
                                totalRetries++;
                                continue;
                            }

                            throw new ServiceCallException(
                                (int)response.StatusCode,
                                error?.message ?? response.ReasonPhrase ?? response.StatusCode.ToString(),
                                typeof(GitHubApiService).Name,
                                httpClient.BaseAddress?.Host ?? "api.github.com",
                                null);
                        }

                        return await response.Content.ReadAsStringAsync().ConfigureAwait(false) ?? "{}";
                    }
                }

                throw new ServiceCallException(
                    500,
                    $"All retries to '{endpoint}' have been exhaused. The call has FAILED due to rate limiting.",
                    typeof(GitHubApiService).Name,
                    httpClient.BaseAddress?.Host ?? "api.github.com",
                    null);
            }

            public static async Task<TResponse?> MakeCall<TResponse>(HttpClient httpClient, string endpoint, string accessToken, params string[] acceptHeaders)
            {
                var content = await MakeCall(httpClient, endpoint, accessToken, acceptHeaders).ConfigureAwait(false)
                    ?? "{}";

                return JsonConvert.DeserializeObject<TResponse>(content);
            }

            private static HttpRequestMessage CreateMessage(string endpoint, string accessToken, params string[] acceptHeaders)
            {
                var message = new HttpRequestMessage(HttpMethod.Get, endpoint);

                message.SetBearerToken(accessToken);

                if (acceptHeaders != null && acceptHeaders.Length > 0)
                {
                    foreach (var header in acceptHeaders)
                    {
                        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(header));
                    }
                }

                return message;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1316:Tuple element names should use correct casing", Justification = "By design.")]
            private static bool IsRecoverableError((HttpResponseMessage response, string? message) error, out TimeSpan waitTime)
            {
                waitTime = TimeSpan.Zero;

                if (error.response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    waitTime = TimeSpan.FromSeconds(1);
                }
                else if (error.response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    if (error.message?.Contains("secondary rate limit") ?? false)
                    {
                        if (int.TryParse(error.response.Headers.RetryAfter?.ToString(), out var seconds) && seconds > 0)
                        {
                            waitTime = TimeSpan.FromSeconds(seconds + 1.25);
                        }
                        else
                        {
                            waitTime = TimeSpan.FromSeconds(30);
                        }
                    }
                }

                return waitTime != TimeSpan.Zero;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "By design.")]
        private sealed class FileSearchResultModel
        {
            public List<ItemModel>? items = new List<ItemModel>();

            internal sealed class ItemModel
            {
                public string? name = null;
                public string? path = null;
                public ItemRepositoryModel? repository = null;

                internal sealed class ItemRepositoryModel
                {
                    public string? name = null;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "By design.")]
        private sealed class RepositorySearchModel
        {
            public string? name = null;
            public string? html_url = null;
            public string? description = null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "By design.")]
        private sealed class CommitSearchModel
        {
            public CommitModel? commit = null;

            internal sealed class CommitModel
            {
                public CommitterModel? committer = null;
            }

            internal sealed class CommitterModel
            {
                public string? name = null;
                public DateTimeOffset? date = null;
            }
        }
    }
}
