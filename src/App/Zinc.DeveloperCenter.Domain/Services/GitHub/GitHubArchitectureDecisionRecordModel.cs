namespace Zinc.DeveloperCenter.Domain.Services.GitHub
{
    /// <summary>
    /// A model used to hold the ADR information.
    /// </summary>
    public class GitHubArchitectureDecisionRecordModel
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

        /// <summary>
        /// Gets the ADR markdown content.
        /// </summary>
        public string? Content { get; set; }
    }
}
