using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Zinc.DeveloperCenter.Application.Services
{
    /// <inheritdoc />
    public class FakeGitHubService : IGitHubService
    {
        /// <summary>
        /// Retrieves the list of Repos for the GSFS GitHub group.
        /// </summary>
        /// <param name="pageNumber"> Page number of GitHub repo query.</param>
        /// <returns> A List of GitHub Repo Records.</returns>
        public async Task<List<GitHubRepoRecord>> GetGitHubRepoData(int pageNumber)
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
                return new List<GitHubRepoRecord>();
            }

            return results;
        }

        /// <summary>
        /// Retrieves the list of Repos for the GSFS GitHub group.
        /// </summary>
        /// <param name="repoDotName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <returns> A List of GitHub Repo Records.</returns>
        public async Task<List<GitHubAdrRecord>> GetGitHubAdrData(string repoDotName)
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
                return new List<GitHubAdrRecord>();
            }

            return results;
        }
    }
}