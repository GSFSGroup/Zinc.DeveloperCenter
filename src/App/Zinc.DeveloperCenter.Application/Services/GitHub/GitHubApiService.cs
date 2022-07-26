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
        public async Task<string> DownloadArchitectureDecisionRecord(
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
                return string.Empty;
            }

            if (fileFormat == FileFormat.Unknown)
            {
                throw new ArgumentException($"The '{nameof(fileFormat)}' argument is invalid.", nameof(fileFormat));
            }

            var orgName = string.IsNullOrEmpty(tenantConfig.OrgName)
                ? tenantConfig.TenantId
                : tenantConfig.OrgName;

            var endpoint = $"repos/{orgName}/{repositoryName}/contents/{filePath}";

            var message = CreateMessage(endpoint, tenantConfig.AccessToken, fileFormat.ToDescription());

            using (var response = await httpClient.SendAsync(message).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeAnonymousType(
                        await response.Content.ReadAsStringAsync().ConfigureAwait(false) ?? "{}",
                        new { message = (string?)null });

                    throw new ServiceCallException(
                        (int)response.StatusCode,
                        error?.message ?? response.ReasonPhrase ?? response.StatusCode.ToString(),
                        GetType().Name,
                        httpClient.BaseAddress?.Host ?? "api.github.com",
                        null);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                return content ?? string.Empty;
            }
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
                return Enumerable.Empty<GitHubArchitectureDecisionRecordModel>();
            }

            var results = await FindArchitectureDecisionRecords(tenantConfig, repositoryName).ConfigureAwait(false);

            if (results.Count == 0)
            {
                var repositoryFullName = string.IsNullOrEmpty(tenantConfig.OrgName)
                    ? $"{tenantConfig.TenantId}/{repositoryName}"
                    : $"{tenantConfig.OrgName}/{repositoryName}";

                logger.LogWarning("Failed to find any ADRs for {Repository}.", repositoryFullName);
            }

            foreach (var result in results)
            {
                var lastUpdatedDetails = await GetLastUpdatedDetails(tenantConfig, repositoryName, result.FilePath).ConfigureAwait(false);

                if (lastUpdatedDetails.LastUpdatedBy?.Length > 0 && lastUpdatedDetails.LastUpdatedOn != null)
                {
                    result.LastUpdatedBy = lastUpdatedDetails.LastUpdatedBy;
                    result.LastUpdatedOn = lastUpdatedDetails.LastUpdatedOn;
                }
            }

            return results;
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
                return Enumerable.Empty<GitHubRepositoryModel>();
            }

            var results = new List<GitHubRepositoryModel>(256);

            var page = 1;
            var pageSize = 100; // 100 is the max

            while (true)
            {
                var retries = 0;
                var repos = await GetRepositories(tenantConfig, page, pageSize).ConfigureAwait(false);

                while (repos.Count == 0 || retries < 3)
                {
                    // Sometimes these api calls fail to return results, so retry 3 times before giving up.
                    if (results.Count == 0)
                    {
                        logger.LogWarning("Failed to find any repositories for {Org}. Retrying...", tenantConfig.OrgName ?? tenantConfig.TenantId);
                    }

                    retries++;

                    repos = await GetRepositories(tenantConfig, page, pageSize).ConfigureAwait(false);
                }

                if (repos.Count == 0)
                {
                    logger.LogWarning("Failed to find any repositories for {Org}. Retries have been exhausted.", tenantConfig.OrgName ?? tenantConfig.TenantId);
                }

                results.AddRange(repos);

                if (repos.Count < pageSize)
                {
                    break;
                }

                page++;
            }

            return results;
        }

        private async Task<List<GitHubArchitectureDecisionRecordModel>> FindArchitectureDecisionRecords(
            GitHubApiConfig.TenantConfig tenantConfig,
            string repositoryName)
        {
            var orgName = string.IsNullOrEmpty(tenantConfig.OrgName)
                ? tenantConfig.TenantId
                : tenantConfig.OrgName;

            // NOTE: This url assumes there will never be more than 100 ADRs in a given app
            var endpoint = $"search/code?q=adr+in:path+language:markdown+org:{orgName}+repo:{orgName}/{repositoryName}&page=1&per_page=100";

            var results = new List<GitHubArchitectureDecisionRecordModel>(32);

            using (var response = await httpClient.SendAsync(CreateMessage(endpoint, tenantConfig.AccessToken)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeAnonymousType(
                        await response.Content.ReadAsStringAsync().ConfigureAwait(false) ?? "{}",
                        new { message = (string?)null });

                    throw new ServiceCallException(
                        (int)response.StatusCode,
                        error?.message ?? response.ReasonPhrase ?? response.StatusCode.ToString(),
                        GetType().Name,
                        httpClient.BaseAddress?.Host ?? "api.github.com",
                        null);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var model = JsonConvert.DeserializeObject<FileSearchResultModel>(content) ?? new FileSearchResultModel();

                // The check for .md or .markdown seems to be unnecessary, but just in case
                var items = model.items?
                    .Where(x => x.path!.EndsWith(".md") || x.path!.EndsWith(".markdown"))
                    .ToList() ?? new List<FileSearchResultModel.ItemModel>();

                if (repositoryName != "Zinc.Templates" && items.Any())
                {
                    // Filters out RedLine template ADRs for applications
                    items = items
                        .Where(x => !x.path!.Contains("docs/RedLine", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                foreach (var item in items)
                {
                    results.Add(new GitHubArchitectureDecisionRecordModel(
                        tenantConfig.TenantId,
                        repositoryName,
                        item.name!,
                        item.path!));
                }
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

            using (var response = await httpClient.SendAsync(CreateMessage(endpoint, tenantConfig.AccessToken)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeAnonymousType(
                        await response.Content.ReadAsStringAsync().ConfigureAwait(false) ?? "{}",
                        new { message = (string?)null });

                    throw new ServiceCallException(
                        (int)response.StatusCode,
                        error?.message ?? response.ReasonPhrase ?? response.StatusCode.ToString(),
                        GetType().Name,
                        httpClient.BaseAddress?.Host ?? "api.github.com",
                        null);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (string.IsNullOrEmpty(content))
                {
                    return default;
                }

                var model = (JsonConvert.DeserializeObject<List<CommitModel>>(content) ?? new List<CommitModel>())
                    .FirstOrDefault();

                if (model != null && model.committer != null)
                {
                    return (model.committer.name, model.committer.date);
                }

                logger.LogWarning("Failed to get commit details for ADR {ADR}.", filePath);
            }

            return default;
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

            using (var response = await httpClient.SendAsync(CreateMessage(endpoint, tenantConfig.AccessToken)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var error = JsonConvert.DeserializeAnonymousType(
                        await response.Content.ReadAsStringAsync().ConfigureAwait(false) ?? "{}",
                        new { message = (string?)null });

                    throw new ServiceCallException(
                        (int)response.StatusCode,
                        error?.message ?? response.ReasonPhrase ?? response.StatusCode.ToString(),
                        GetType().Name,
                        httpClient.BaseAddress?.Host ?? "api.github.com",
                        null);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var results = (JsonConvert.DeserializeObject<List<RepositorySearchModel>>(content) ?? new List<RepositorySearchModel>())
                    .Select(model => new GitHubRepositoryModel(tenantConfig.TenantId!, model.name!, model.html_url!, model.description))
                    .ToList();

                return results;
            }
        }

        private HttpRequestMessage CreateMessage(string endpoint, string accessToken, params string[] acceptHeaders)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "By design.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "By design.")]
        private sealed class FileSearchResultModel
        {
            public List<ItemModel>? items = new List<ItemModel>();

            internal sealed class ItemModel
            {
                public string? name = null;
                public string? path = null;
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
        private sealed class CommitModel
        {
            public CommitterModel? committer = null;

            internal sealed class CommitterModel
            {
                public string? name = null;
                public DateTimeOffset? date = null;
            }
        }
    }
}
