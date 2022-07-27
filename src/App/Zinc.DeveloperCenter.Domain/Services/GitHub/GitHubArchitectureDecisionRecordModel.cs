using System;

namespace Zinc.DeveloperCenter.Domain.Services.GitHub
{
    /// <summary>
    /// A model used to hold ADR information.
    /// </summary>
    public sealed class GitHubArchitectureDecisionRecordModel : IEquatable<GitHubArchitectureDecisionRecordModel>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="filePath">The ADR file path.</param>
        public GitHubArchitectureDecisionRecordModel(
            string tenantId,
            string applicationName,
            string filePath)
        {
            TenantId = tenantId;
            ApplicationName = applicationName;
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
        public string FileName => System.IO.Path.GetFileName(FilePath);

        /// <summary>
        /// Gets the file path.
        /// </summary>
        public string FilePath { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as GitHubArchitectureDecisionRecordModel);
        }

        /// <inheritdoc/>
        public bool Equals(GitHubArchitectureDecisionRecordModel? other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return FileName.Equals(other.FileName);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return FileName.GetHashCode();
        }
    }
}
