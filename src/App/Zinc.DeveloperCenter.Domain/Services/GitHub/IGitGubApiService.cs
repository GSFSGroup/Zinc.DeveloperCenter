using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zinc.DeveloperCenter.Domain.Services.GitHub
{
    /// <summary>
    /// The interface that is used to interact with GitHub.
    /// </summary>
    public interface IGitGubApiService
    {
        /// <summary>
        /// Gets the architecture decision records defined in a repository.
        /// </summary>
        /// <param name="repositoryName">The repository name.</param>
        /// <param name="includeContent">If true, the ADR content will be returned as well.</param>
        /// <returns>A collections of <see cref="GitHubArchitectureDecisionRecordModel"/>s.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "By design.")]
        Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> GetArchitectureDecisionRecords(
            string repositoryName,
            bool includeContent = false);

        /// <summary>
        /// Gets the collection of repositories defined for the organization.
        /// </summary>
        /// <returns>A collection of <see cref="GitHubRepositoryModel"/>s.</returns>
        Task<IEnumerable<GitHubRepositoryModel>> GetRepositories();
    }
}
