using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;
using Zinc.DeveloperCenter.Application.Queries.GitHubRepo.Models;

namespace Zinc.DeveloperCenter.Application.Services
{
    /// <summary>
    /// A service for calling endpoints in the GitHub API.
    /// </summary>
    public interface IGitHubService
    {
        /// <summary>
        /// Retrieves the list of Repos for the GSFS GitHub group.
        /// </summary>
        /// <param name="pageNumber"> Page number of GitHub repo query.</param>
        /// <returns> A List of GitHub Repo Records.</returns>
        Task<List<GitHubRepoModel>> GetGitHubRepoData(int pageNumber);

        /// <summary>
        /// Retrieves the list of Adrs for a specific Repo in the GSFS group.
        /// </summary>
        /// <param name="repoDotName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <returns> A List of Adrs from a specific GSFS group repo.</returns>
        Task<List<GitHubAdrSummaryModel>> GetGitHubAdrData(string repoDotName);

        /// <summary>
        /// Retrieves the time at which a specific Adr was last updated.
        /// </summary>
        /// <param name="repoDotName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <param name="adrTitle"> Full title of Adr. ex: adr-0001-full-adr-name.md.</param>
        /// <returns> A string of the date on which the Adr was most recently updated.</returns>
        Task<DateTime> GetAdrLastUpdatedData(string repoDotName, string adrTitle);
    }
}