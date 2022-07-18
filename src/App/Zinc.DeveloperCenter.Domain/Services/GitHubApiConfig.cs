using System.Collections.Generic;

namespace Zinc.DeveloperCenter.Domain.Services
{
    /// <summary>
    /// The configuration settings needed by the <see cref="IGitGubApi"/>.
    /// </summary>
    public class GitHubApiConfig
    {
        /// <summary>
        /// The configuration sectino name.
        /// </summary>
        public static readonly string SectionName = "GitHubApi";

        /// <summary>
        /// The base url for the API.
        /// </summary>
        public string? BaseUrl { get; set; }

        /// <summary>
        /// Token to allow access to private repos.
        /// </summary>
        public string? AccessToken { get; set; }

        /// <summary>
        /// If true, a fake IGitHubApi will be used (useful for local testing).
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Dictionary of directories where ADRs are stored in a given repository. The dictionary is indexed by repository name.
        /// </summary>
        public Dictionary<string, string> AdrDirectoryUrls { get; set; } = new Dictionary<string, string>();
    }
}
