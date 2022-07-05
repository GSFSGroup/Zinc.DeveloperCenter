using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Zinc.DeveloperCenter.Application.Services
{
    /// <inheritdoc />
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<GitHubServiceConfig> gitHubServiceConfig;

        /// <summary>
        /// Initializes an instance of <see cref="GitHubService"/>.
        /// </summary>
        /// <param name="httpClient">The http client to make requests.</param>.
        /// <param name="gitHubServiceConfig">The configuraton settings for <see cref="GitHubServiceConfig"/>.</param>.
        public GitHubService(
            HttpClient httpClient,
            IOptions<GitHubServiceConfig> gitHubServiceConfig)
        {
            this.httpClient = httpClient;
            this.gitHubServiceConfig = gitHubServiceConfig;
        }

        /// <summary>
        /// Retrieves the list of Repos for the GSFS GitHub group.
        /// </summary>
        /// <param name="pageNumber"> Page number of GitHub repo query.</param>
        /// <returns> A List of GitHub Repo Records.</returns>
        public async Task<List<GitHubRepoRecord>> GetGitHubRepoData(int pageNumber)
        {
            var config = gitHubServiceConfig.Value;
            var pathUrl = $"/orgs/GSFSGroup/repos?per_page=100&page=";
            pathUrl = string.Concat(pathUrl, pageNumber.ToString());
            var uriBuilder = new UriBuilder($"{config.BaseUrl}{pathUrl}");
            var response = await httpClient.SendAsync(CreateMessage(uriBuilder.ToString())).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var results = JsonConvert.DeserializeObject<List<GitHubRepoRecord>>(responseContent);

            if (results == null || results.Count == 0)
            {
                return new List<GitHubRepoRecord>();
            }

            return results;
        }

        /// <summary>
        /// Retrieves the list of Adrs for a specific Repo in the GSFS group.
        /// </summary>
        /// <param name="repoDotName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <returns> A List of Adrs from a specific GSFS group repo.</returns>
        public async Task<List<GitHubAdrRecord>> GetGitHubAdrData(string repoDotName)
        {
            var config = gitHubServiceConfig.Value;
            var pathUrl = $"/repos/GSFSGroup/{repoDotName}/contents/docs/App?per_page=60";

            if (config.AdrDirectoryUrls.ContainsKey(repoDotName))
            {
                pathUrl = $"/repos/GSFSGroup/{repoDotName}/contents/{config.AdrDirectoryUrls[repoDotName]}?per_page=60";
            }

            var uriBuilder = new UriBuilder($"{config.BaseUrl}{pathUrl}");

            var response = await httpClient.SendAsync(CreateMessage(uriBuilder.ToString())).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var results = JsonConvert.DeserializeObject<List<GitHubAdrRecord>>(responseContent);

            if (results == null || results.Count == 0)
            {
                return new List<GitHubAdrRecord>();
            }

            return results;
        }

        private HttpRequestMessage CreateMessage(string endpoint)
        {
            var config = gitHubServiceConfig.Value;
            var message = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var accessToken = Environment.ExpandEnvironmentVariables(config.AccessToken);

            message.SetBearerToken(accessToken);
            var productValue = new ProductInfoHeaderValue("RedLine", "1.0");
            message.Headers.UserAgent.Add(productValue);

            return message;
        }
    }
}