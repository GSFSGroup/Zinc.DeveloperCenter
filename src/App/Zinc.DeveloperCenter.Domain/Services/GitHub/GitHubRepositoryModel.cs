namespace Zinc.DeveloperCenter.Domain.Services.GitHub
{
    /// <summary>
    /// A model used to hold the repository information.
    /// </summary>
    public sealed class GitHubRepositoryModel : System.IEquatable<GitHubRepositoryModel>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="applicationUrl">The application url.</param>
        /// <param name="applicationDescription">The application description.</param>
        public GitHubRepositoryModel(
            string tenantId,
            string applicationName,
            string applicationUrl,
            string? applicationDescription)
        {
            var appName = AppName.Parse(applicationName);

            TenantId = tenantId;
            ApplicationName = appName.ApplicationName;
            ApplicationDisplayName = appName.ApplicationDisplayName;
            ApplicationElement = appName.ApplicationElement;
            ApplicationUrl = applicationUrl;
            ApplicationDescription = applicationDescription;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public GitHubRepositoryModel()
        {
            TenantId = string.Empty;
            ApplicationName = string.Empty;
            ApplicationDisplayName = string.Empty;
            ApplicationElement = string.Empty;
            ApplicationUrl = string.Empty;
            ApplicationDescription = string.Empty;
        }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// The application element name.
        /// </summary>
        public string? ApplicationElement { get; set; }

        /// <summary>
        /// The application name.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The application display name.
        /// </summary>
        public string ApplicationDisplayName { get; set; }

        /// <summary>
        /// The application description.
        /// </summary>
        public string? ApplicationDescription { get; set; }

        /// <summary>
        /// The application url.
        /// </summary>
        public string ApplicationUrl { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as GitHubRepositoryModel);
        }

        /// <inheritdoc/>
        public bool Equals(GitHubRepositoryModel? other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return TenantId.Equals(other.TenantId) && ApplicationName.Equals(other.ApplicationName);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return TenantId.GetHashCode() ^ ApplicationName.GetHashCode();
        }
    }
}
