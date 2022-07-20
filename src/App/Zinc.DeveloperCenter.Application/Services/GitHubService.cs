using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;
using Zinc.DeveloperCenter.Application.Queries.GitHubRepo.Models;

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
        /// Retrieves a list of all pages of Repos for the GSFS GitHub group.
        /// </summary>
        /// <returns> A List of GitHub Repo Records.</returns>
        public async Task<List<GitHubRepoModel>> GetGitHubRepoData()
        {
            int page = 1;

            var repoList = await this
                .GetGitHubRepoDataPage(page)
                .ConfigureAwait(false);

            // pagination solution.
            // GitHub can query 100 repos at a time.
            // This repeatedly grabs 100 repos until reaching the last page.
            while (true)
            {
                page++;

                var recordToAppend = await this
                    .GetGitHubRepoDataPage(page)
                    .ConfigureAwait(false);

                repoList.AddRange(recordToAppend);

                if (recordToAppend.Count < 100)
                {
                    break;
                }
            }

            return repoList;
        }

        /// <summary>
        /// Retrieves the list of Repos for the GSFS GitHub group.
        /// </summary>
        /// <param name="pageNumber"> Page number of GitHub repo query.</param>
        /// <returns> A List of GitHub Repo Records.</returns>
        public async Task<List<GitHubRepoModel>> GetGitHubRepoDataPage(int pageNumber)
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
                return new List<GitHubRepoModel>();
            }

            var repoList = new List<GitHubRepoModel>();

            foreach (var repoRecord in results.Select(x => x.Name))
            {
                var nameParts = repoRecord.Split('.');
                var element = nameParts[0];
                var neatName = string.Join(".", nameParts.Skip(1));

                // a few repos do not contain periods,
                // and their neatName will be stored as their element.
                // this swaps the two strings for such repos.
                if (string.IsNullOrEmpty(neatName))
                {
                    neatName = repoRecord;
                    element = string.Empty;
                }

                var repo = new GitHubRepoModel
                {
                    DotName = repoRecord,
                    NeatName = neatName,
                    Element = element,
                };

                repoList.Add(repo);
            }

            return repoList;
        }

        /// <summary>
        /// Retrieves the list of Adrs for a specific Repo in the GSFS group.
        /// </summary>
        /// <param name="repoDotName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <returns> A List of Adrs from a specific GSFS group repo.</returns>
        public async Task<List<GitHubAdrSummaryModel>> GetGitHubAdrData(string repoDotName)
        {
            // getting adr data from GitHub
            var config = gitHubServiceConfig.Value;
            var pathUrl = $"/repos/GSFSGroup/{repoDotName}/contents/docs/App?per_page=60";

            if (config.AdrDirectoryUrls.ContainsKey(repoDotName))
            {
                pathUrl = $"/repos/GSFSGroup/{repoDotName}/contents/{config.AdrDirectoryUrls[repoDotName]}?per_page=60";
            }

            var uriBuilder = new UriBuilder($"{config.BaseUrl}{pathUrl}");

            var response = await httpClient.SendAsync(CreateMessage(uriBuilder.ToString())).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<GitHubAdrSummaryModel>();
            }

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var results = JsonConvert.DeserializeObject<List<GitHubAdrRecord>>(responseContent);

            if (results == null || results.Count == 0)
            {
                return new List<GitHubAdrSummaryModel>();
            }

            // converting adr data to a list of adr summary models
            var adrsToReturn = new List<GitHubAdrSummaryModel>();

            foreach (var adrRecord in results)
            {
                // add a file to the list if it is an adr.
                // an adr will begin with "adr" and end with ".md" or ".markdown".
                if ((adrRecord.Name.Length > 4 && adrRecord.Name.Substring(0, 3) == "adr") && (adrRecord.Name.Substring(adrRecord.Name.Length - 3) == ".md" || (adrRecord.Name.Length > 10 && adrRecord.Name.Substring(adrRecord.Name.Length - 9) == ".markdown")))
                {
                    int indexSecondDash = adrRecord.Name.IndexOf('-', adrRecord.Name.IndexOf('-') + 1);
                    var nameParts = adrRecord.Name.Split('-');

                    var adr = new GitHubAdrSummaryModel
                    {
                        NeatTitle = adrRecord.Name.Substring(indexSecondDash, adrRecord.Name.IndexOf('.') - indexSecondDash).Replace('-', ' '),
                        AdrTitle = adrRecord.Name,
                        LastUpdatedDate = string.Empty,
                        Number = Convert.ToInt16(nameParts[1]),
                        NumberString = string.Concat(nameParts[0], '-', nameParts[1]),
                        DownloadUrl = adrRecord.DownloadUrl,
                        HtmlUrl = adrRecord.HtmlUrl,
                    };

                    adrsToReturn.Add(adr);
                }
            }

            return adrsToReturn;
        }

        /// <summary>
        /// Retrieves the time at which a specific Adr was last updated.
        /// </summary>
        /// <param name="repoDotName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <param name="adrTitle"> Full title of Adr. ex: adr-0001-full-adr-name.md.</param>
        /// <returns> A string of the date on which the Adr was most recently updated.</returns>
        public async Task<DateTime> GetAdrLastUpdatedData(string repoDotName, string adrTitle)
        {
            var config = gitHubServiceConfig.Value;
            var pathUrl = $"/repos/GSFSGroup/{repoDotName}/commits?path={config.AdrDirectoryUrls[repoDotName]}/{adrTitle}&page=1&per_page=1";

            if (!config.AdrDirectoryUrls.ContainsKey(repoDotName))
            {
                return new DateTime(2015, 12, 25);
            }

            var uriBuilder = new UriBuilder($"{config.BaseUrl}{pathUrl}");

            var response = await httpClient.SendAsync(CreateMessage(uriBuilder.ToString())).ConfigureAwait(false);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new DateTime(2015, 12, 25);
            }

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var results = JsonConvert.DeserializeObject<dynamic>(responseContent);

            if (results != null)
            {
                return results[0].commit.committer.date;
            }

            return new DateTime(2015, 12, 25);
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