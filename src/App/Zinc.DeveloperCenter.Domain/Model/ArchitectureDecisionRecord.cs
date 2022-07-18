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
        /// <param name="applicationElement">The application element name.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="applicationDisplayName">The application display name.</param>
        /// <param name="title">The ADR title.</param>
        /// <param name="number">The ADR number.</param>
        /// <param name="lastUpdated">The ADR last updated date.</param>
        /// <param name="downloadUrl">The ADR content url.</param>
        /// <param name="content">The raw ADR markdown content.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "By design.")]
        public ArchitectureDecisionRecord(
            string applicationElement,
            string applicationName,
            string applicationDisplayName,
            string title,
            string number,
            string lastUpdated,
            string downloadUrl,
            string? content = null)
        {
            ApplicationElement = applicationElement;
            ApplicationName = applicationName;
            ApplicationDisplayName = applicationDisplayName;
            Title = title;
            Number = number;
            LastUpdated = lastUpdated;
            DownloadUrl = downloadUrl;

            if (!string.IsNullOrEmpty(content))
            {
                Content = content;
            }
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected ArchitectureDecisionRecord()
        { }

        /// <summary>
        /// Gets the application element name.
        /// </summary>
        public string? ApplicationElement { get; protected set; }

        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string? ApplicationName { get; protected set; }

        /// <summary>
        /// Gets the application display name where the ADR is defined.
        /// </summary>
        public string? ApplicationDisplayName { get; protected set; }

        /// <summary>
        /// Gets the ADR title.
        /// </summary>
        public string? Title { get; protected set; }

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public string? Number { get; protected set; }

        /// <summary>
        /// Gets the ADR last updated date.
        /// </summary>
        public string? LastUpdated { get; protected set; }

        /// <summary>
        /// Gets the ADR raw markdown content.
        /// </summary>
        public string? Content { get; protected set; }

        /// <summary>
        /// Gets the ADR download url.
        /// </summary>
        public string? DownloadUrl { get; protected set; }

        /// <inheritdoc/>
        public override string Key => $"{ApplicationName}/{Number}";

        /// <summary>
        /// Gets surrogate the id for the ADR.
        /// </summary>
        internal int Sid { get; set; }

        /// <summary>
        /// Updates the ADR title.
        /// </summary>
        /// <param name="newTitle">The new title.</param>
        public void UpdateTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
            {
                throw new ArgumentException($"The {nameof(newTitle)} argument is required.", nameof(newTitle));
            }

            Title = newTitle;
        }

        /// <summary>
        /// Updates the ADR last updated date.
        /// </summary>
        /// <param name="lastUpdated">The last updated date.</param>
        public void UpdateLastUpdated(string lastUpdated)
        {
            if (string.IsNullOrWhiteSpace(lastUpdated))
            {
                throw new ArgumentException($"The {nameof(lastUpdated)} argument is required.", nameof(lastUpdated));
            }

            LastUpdated = lastUpdated;
        }

        /// <summary>
        /// Updates the ADR content.
        /// </summary>
        /// <param name="content">The raw markdown content.</param>
        public void UpdateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException($"The {nameof(content)} argument is required.", nameof(content));
            }

            Content = content;
        }

        /// <summary>
        /// Updates the ADR content url.
        /// </summary>
        /// <param name="downloadUrl">The content url.</param>
        public void UpdateDownloadUrl(string downloadUrl)
        {
            if (string.IsNullOrWhiteSpace(downloadUrl))
            {
                throw new ArgumentException($"The {nameof(downloadUrl)} argument is required.", nameof(downloadUrl));
            }

            DownloadUrl = downloadUrl;
        }
    }
}
