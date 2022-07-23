using System;

namespace Zinc.DeveloperCenter.Domain.Model.GitHub
{
    /// <summary>
    /// A model used to hold ADR information.
    /// </summary>
    public class GitHubArchitectureDecisionRecordModel
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="fileName">The ADR file name.</param>
        /// <param name="filePath">The ADR file path.</param>
        public GitHubArchitectureDecisionRecordModel(
            string tenantId,
            string applicationName,
            string fileName,
            string filePath)
        {
            TenantId = tenantId;
            ApplicationName = applicationName;
            FileName = fileName;
            FilePath = filePath;
        }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets the ADR title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets the user who last updated the ADR.
        /// </summary>
        public string? LastUpdatedBy { get; set; }

        /// <summary>
        /// Gets the ADR last updated date.
        /// </summary>
        public DateTime? LastUpdatedOn { get; set; }

        /// <summary>
        /// Gets the ADR download url.
        /// </summary>
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Gets the url used to view the ADR on GitHub.
        /// </summary>
        public string? HtmlUrl { get; set; }
    }
}
