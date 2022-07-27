using System;
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
        /// Downloads a file.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="repositoryName">The name of the repository.</param>
        /// <param name="filePath">The GitHub file path.</param>
        /// <param name="fileFormat">The <see cref="FileFormat"/> to return.</param>
        /// <returns>The raw contents of the file to download.</returns>
        Task<string> DownloadArchitectureDecisionRecord(
            string tenantId,
            string repositoryName,
            string filePath,
            FileFormat fileFormat);

        /// <summary>
        /// Finds the architecture decision records defined in an organization.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="page">The page to return.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>A collections of <see cref="GitHubArchitectureDecisionRecordModel"/>s.</returns>
        Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> FindArchitectureDecisionRecords(string tenantId, int page, int pageSize);

        /// <summary>
        /// Gets the last updated details for an ADR.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="repositoryName">The name of the repository.</param>
        /// <param name="filePath">The GitHub file path.</param>
        /// <returns>The last update by and last updated on details.</returns>
        Task<(string? LastUpdatedBy, DateTimeOffset? LastUpdatedOn)> GetLastUpdatedDetails(
            string tenantId,
            string repositoryName,
            string filePath);

        /// <summary>
        /// Gets the collection of repositories defined for the organization.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns>A collection of <see cref="GitHubRepositoryModel"/>s.</returns>
        Task<IEnumerable<GitHubRepositoryModel>> GetRepositories(string tenantId);
    }
}
