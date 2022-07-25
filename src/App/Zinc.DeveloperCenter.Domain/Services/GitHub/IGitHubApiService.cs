using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zinc.DeveloperCenter.Domain.Model.GitHub
{
    /// <summary>
    /// The interface that is used to interact with GitHub.
    /// </summary>
    public interface IGitHubApiService
    {
        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="repositoryName">The name of the repository.</param>
        /// <param name="filePath">The download url.</param>
        /// <param name="fileFormat">The <see cref="FileFormat"/> to return.</param>
        /// <returns>The raw contents of the file to download.</returns>
        Task<string> DownloadArchitectureDecisionRecord(
            string tenantId,
            string repositoryName,
            string filePath,
            FileFormat fileFormat);

        /// <summary>
        /// Gets the architecture decision records defined in a repository.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="repositoryName">The repository to search.</param>
        /// <returns>A collections of <see cref="GitHubArchitectureDecisionRecordModel"/>s.</returns>
        Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> FindArchitectureDecisionRecords(string tenantId, string repositoryName);

        /// <summary>
        /// Gets the collection of repositories defined for the organization.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns>A collection of <see cref="GitHubRepositoryModel"/>s.</returns>
        Task<IEnumerable<GitHubRepositoryModel>> GetRepositories(string tenantId);
    }
}
