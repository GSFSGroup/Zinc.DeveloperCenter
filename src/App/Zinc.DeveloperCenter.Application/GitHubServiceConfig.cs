namespace Zinc.DeveloperCenter.Application
{
    /// <summary>
    /// The configuration settings for accessing the GitHubService.
    /// </summary>
    public class GitHubServiceConfig
    {
        /// <summary>
        /// The GitHub API Token to pull for <see cref="GitHubServiceConfig"/>.
        /// </summary>
        public static readonly string SectionName = "GitHubApi";

        /// <summary>
        /// Initializes instance of a GitHubServiceConfig.
        /// </summary>
        public GitHubServiceConfig()
        {
            BaseUrl = string.Empty;
            AccessToken = string.Empty;
        }

        /// <summary>
        /// The base url for the API.
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Token to allow access to private repos.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Enables the HTTP calls to GitHubServiceConfig. If disabled, a fake service is used.
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}