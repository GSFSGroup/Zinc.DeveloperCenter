using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;
using Zinc.DeveloperCenter.Application.Queries.GitHubRepo.Models;

namespace Zinc.DeveloperCenter.Application.Services
{
    /// <inheritdoc />
    public class FakeGitHubService : IGitHubService
    {
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
            var resource = $"{typeof(FakeGitHubService).Namespace}.GitHubApiGetReposResponse.json";
            await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);

            if (stream == null)
            {
                throw new InvalidOperationException($"failed to load embedded resource '${resource}'.");
            }

            using var textStreamReader = new StreamReader(stream);

            var json = await textStreamReader.ReadToEndAsync()
                .ConfigureAwait(false);

            var results = JsonConvert.DeserializeObject<List<GitHubRepoRecord>>(json);

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
                    ApplicationName = repoRecord,
                    ApplicationDisplayName = neatName,
                    ApplicationElement = element,
                };

                repoList.Add(repo);
            }

            List<GitHubRepoModel> sortedRepoList = repoList.OrderBy(o => o.ApplicationDisplayName).ToList();

            return sortedRepoList;
        }

        /// <summary>
        /// Retrieves the time at which a specific Adr was last updated.
        /// </summary>
        /// <param name="applicationName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <param name="adrTitle"> Full title of Adr. ex: adr-0001-full-adr-name.md.</param>
        /// <returns> A string of the date on which the Adr was most recently updated.</returns>
        public async Task<DateTime> GetAdrLastUpdatedData(string applicationName, string adrTitle)
        {
            var resource = $"{typeof(FakeGitHubService).Namespace}.GitHubApiGetLastUpdatedDateResponse.json";
            await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);

            if (stream == null)
            {
                throw new InvalidOperationException($"failed to load embedded resource '${resource}'.");
            }

            using var textStreamReader = new StreamReader(stream);

            var json = await textStreamReader.ReadToEndAsync()
                .ConfigureAwait(false);

            var results = JsonConvert.DeserializeObject<dynamic>(json);

            if (results == null)
            {
                return new DateTime(2015, 12, 25);
            }

            return results;
        }

        /// <summary>
        /// Retrieves the list of Repos for the GSFS GitHub group.
        /// </summary>
        /// <param name="applicationName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <returns> A List of GitHub Repo Records.</returns>
        public async Task<List<GitHubAdrSummaryModel>> GetGitHubAdrData(string applicationName)
        {
            var resource = $"{typeof(FakeGitHubService).Namespace}.GitHubApiGetAdrsResponse.json";
            await using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);

            if (stream == null)
            {
                throw new InvalidOperationException($"failed to load embedded resource '${resource}'.");
            }

            using var textStreamReader = new StreamReader(stream);

            var json = await textStreamReader.ReadToEndAsync()
                .ConfigureAwait(false);

            var results = JsonConvert.DeserializeObject<List<GitHubAdrRecord>>(json);

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
    }
}