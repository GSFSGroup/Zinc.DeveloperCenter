using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zinc.DeveloperCenter.Domain.Services.GitHub
{
    /// <summary>
    /// The interface that is used to interact with GitHub.
    /// </summary>
    public interface IGitHubApiService
    {
        /// <summary>
        /// Downloads the ADR markdown.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="downloadUrl">The download url.</param>
        /// <returns>The raw ADR markdown content.</returns>
        Task<string> DownloadArchitectureDecisionRecord(string tenantId, string downloadUrl);

        /// <summary>
        /// Gets the architecture decision records defined in a repository.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="repositoryName">The repository name.</param>
        /// <returns>A collections of <see cref="GitHubArchitectureDecisionRecordModel"/>s.</returns>
        Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> GetArchitectureDecisionRecords(string tenantId, string repositoryName);

        /// <summary>
        /// Gets the collection of repositories defined for the organization.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns>A collection of <see cref="GitHubRepositoryModel"/>s.</returns>
        Task<IEnumerable<GitHubRepositoryModel>> GetRepositories(string tenantId);
    }
}
