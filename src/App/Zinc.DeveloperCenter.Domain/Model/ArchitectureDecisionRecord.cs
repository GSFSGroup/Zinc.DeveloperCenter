using System;
using RedLine.Domain.Model;

namespace Zinc.DeveloperCenter.Domain.Model
{
    /// <summary>
    /// Represents an architecture decision record.
    /// </summary>
    public class ArchitectureDecisionRecord : AggregateRootBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="filePath">The ADR file path in GitHub.</param>
        /// <param name="lastUpdatedBy">The user who last updated the ADR.</param>
        /// <param name="lastUpdatedOn">The date when the ADR was last updated.</param>
        /// <param name="content">The raw ADR markdown content.</param>
        public ArchitectureDecisionRecord(
            string tenantId,
            string applicationName,
            string filePath,
            string? lastUpdatedBy,
            DateTimeOffset? lastUpdatedOn,
            string? content)
        {
            TenantId = tenantId;
            ApplicationName = applicationName;
            FilePath = filePath;
            LastUpdatedBy = lastUpdatedBy;
            LastUpdatedOn = lastUpdatedOn;
            Content = content;
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected ArchitectureDecisionRecord()
        { }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        public string TenantId { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string ApplicationName { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets the file path in GitHub.
        /// </summary>
        public string FilePath { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName => System.IO.Path.GetFileName(FilePath);

        /// <summary>
        /// Gets the ADR title.
        /// </summary>
        public string Title => System.IO.Path.GetFileNameWithoutExtension(FilePath).Split('-', 3)[2];

        /// <summary>
        /// Gets the ADR title display name.
        /// </summary>
        public string TitleDisplay => Title.Replace('-', ' ');

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public int Number => int.Parse(FileName.Split('-')[1]);

        /// <summary>
        /// Gets the ADR number display format (adr-0001).
        /// </summary>
        public string NumberDisplay => string.Format($"adr-{0}", Number.ToString("0000"));

        /// <summary>
        /// Gets the user who last updated the ADR.
        /// </summary>
        public string? LastUpdatedBy { get; protected set; }

        /// <summary>
        /// Gets the ADR last updated date.
        /// </summary>
        public DateTimeOffset? LastUpdatedOn { get; protected set; }

        /// <summary>
        /// Gets the ADR raw markdown content.
        /// </summary>
        public string? Content { get; protected set; }

        /// <inheritdoc/>
        public override string Key => $"{TenantId}/{ApplicationName}/{FilePath}";

        /// <summary>
        /// Gets the total views of the ADR.
        /// </summary>
        public int TotalViews { get; protected set; }

        /// <summary>
        /// Updates the ADR last updated date.
        /// </summary>
        /// <param name="updatedBy">The user who last updated the ADR.</param>
        /// <param name="updatedOn">The date the ADR was last updated.</param>
        public void UpdateLastUpdated(string? updatedBy, DateTimeOffset? updatedOn)
        {
            if (LastUpdatedBy != updatedBy || LastUpdatedOn != updatedOn)
            {
                LastUpdatedBy = updatedBy;
                LastUpdatedOn = updatedOn;
            }
        }

        /// <summary>
        /// Updates the ADR content.
        /// </summary>
        /// <param name="content">The raw markdown content.</param>
        public void UpdateContent(string content)
        {
            if (content.Length == 0)
            {
                throw new ArgumentException($"The {nameof(content)} argument is required.", nameof(content));
            }

            Content = content;
        }
    }
}
