using System.Collections.Generic;

namespace Zinc.DeveloperCenter.Domain.Services.GitHub
{
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
    }
}
