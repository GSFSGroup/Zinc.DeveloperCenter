using System.Collections.Generic;
using System.Threading.Tasks;

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
        Task<List<GitHubRepoRecord>> GetGitHubRepoData(int pageNumber);

        /// <summary>
        /// Retrieves the list of Adrs for a specific Repo in the GSFS group.
        /// </summary>
        /// <param name="isTemplate">Query RedLine Adrs only if querying for Zinc.Templates.</param>
        /// <returns> A List of Adrs from a specific GSFS group repo.</returns>
        Task<List<GitHubAdrRecord>> GetGitHubAdrData(bool isTemplate);
    }
}