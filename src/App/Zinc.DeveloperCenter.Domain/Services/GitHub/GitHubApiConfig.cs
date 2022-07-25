using System.Collections.Generic;

namespace Zinc.DeveloperCenter.Domain.Model.GitHub
{
    /// <summary>
    /// The configuration settings needed by the <see cref="IGitHubApiService"/>.
    /// </summary>
    public class GitHubApiConfig
    {
        /// <summary>
        /// The configuration section name.
        /// </summary>
        public static readonly string SectionName = "GitHubApi";

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GitHubApiConfig()
        {
            Tenants = new List<TenantConfig>();
        }

        /// <summary>
        /// Gets the collection of configured tenants.
        /// </summary>
        public List<TenantConfig> Tenants { get; set; }

        /// <summary>
        /// Represents a configured tenant.
        /// </summary>
        public class TenantConfig
        {
            private string accessToken = string.Empty;

            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            public TenantConfig()
            {
                TenantId = string.Empty;
                OrgName = string.Empty;
                AccessToken = string.Empty;
            }

            /// <summary>
            /// Gets the tenant identifier.
            /// </summary>
            public string TenantId { get; set; }

            /// <summary>
            /// Gets the GitHub organization name.
            /// </summary>
            public string? OrgName { get; set; }

            /// <summary>
            /// Token to allow access to private repos.
            /// </summary>
            public string AccessToken
            {
                get => System.Environment.ExpandEnvironmentVariables(this.accessToken);
                set => this.accessToken = value;
            }

            /// <summary>
            /// If true, the tenant will not be processed.
            /// </summary>
            public bool Disabled { get; set; }
        }
    }
}
