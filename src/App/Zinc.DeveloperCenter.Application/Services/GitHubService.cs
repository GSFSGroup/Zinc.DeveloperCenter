using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json;

namespace Zinc.DeveloperCenter.Application.Services.GitHubService
{
    /// <inheritdoc />
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes an instance of <see cref="GitHubService"/>.
        /// </summary>
        /// <param name="httpClient">The http client to make requests.</param>.
        public GitHubService(
            HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves the list of Repos for the GSFS GitHub group.
        /// </summary>
        /// <param name="pageNumber"> Page number of GitHub repo query.</param>
        /// <returns> A List of GitHub Repo Records.</returns>
        public async Task<List<GitHubRepoRecord>> GetGitHubRepoData(int pageNumber)
        {
            var pathUrl = $"https://api.github.com/orgs/GSFSGroup/repos?per_page=100&page=";
            pathUrl = string.Concat(pathUrl, pageNumber.ToString());
            var uriBuilder = new UriBuilder(pathUrl);
            var response = await httpClient.SendAsync(CreateMessage(uriBuilder.ToString())).ConfigureAwait(false);

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var results = JsonConvert.DeserializeObject<List<GitHubRepoRecord>>(responseContent);

            if (results == null || results.Count == 0)
            {
                return new List<GitHubRepoRecord>();
            }

            return results;
        }

        private HttpRequestMessage CreateMessage(string endpoint)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, endpoint);

            message.SetBearerToken("ghp_");
            var productValue = new ProductInfoHeaderValue("RedLine", "1.0");
            message.Headers.UserAgent.Add(productValue);

            return message;
        }
    }
}