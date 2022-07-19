using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zinc.DeveloperCenter.Domain.Services
{
    /// <summary>
    /// The interface that is used to interact with GitHub.
    /// </summary>
    public interface IGitGubApi
    {
        /// <summary>
        /// Gets the architecture decision records defined in a repository.
        /// </summary>
        /// <param name="repositoryName">The repository name.</param>
        /// <returns>A collections of <see cref="ArchitectureDecisionRecordModel"/>s.</returns>
        Task<IEnumerable<ArchitectureDecisionRecordModel>> GetArchitectureDecisionRecords(string repositoryName);

        /// <summary>
        /// Gets the collection of repositories defined for the organization.
        /// </summary>
        /// <returns>A collection of <see cref="GitHubRepositoryModel"/>s.</returns>
        Task<IEnumerable<GitHubRepositoryModel>> GetRepositories();
    }

    /// <summary>
    /// A model used to hold the repository information.
    /// </summary>
    public class GitHubRepositoryModel
    {
        /// <summary>
        /// The application element name.
        /// </summary>
        public string? ApplicationElement { get; set; }

        /// <summary>
        /// The application name.
        /// </summary>
        public string? ApplicationName { get; set; }

        /// <summary>
        /// The application display name.
        /// </summary>
        public string? ApplicationDisplayName { get; set; }

        /// <summary>
        /// Gets the collection of ADRs defined in the repository.
        /// </summary>
        public List<ArchitectureDecisionRecordModel> ArchitectureDecisionRecords { get; set; } = new List<ArchitectureDecisionRecordModel>(32);
    }

    /// <summary>
    /// A model used to hold the ADR information.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "By design.")]
    public class ArchitectureDecisionRecordModel
    {
        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string? ApplicationName { get; set; }

        /// <summary>
        /// Gets the ADR title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets the ADR last updated date.
        /// </summary>
        public string? LastUpdated { get; set; }

        /// <summary>
        /// Gets the ADR download url.
        /// </summary>
        public string? DownloadUrl { get; set; }
    }
}
