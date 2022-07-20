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
        /// <param name="applicationName">The application name.</param>
        /// <param name="number">The ADR number.</param>
        /// <param name="title">The ADR title.</param>
        /// <param name="lastUpdated">The ADR last updated date.</param>
        /// <param name="downloadUrl">The ADR content url.</param>
        /// <param name="htmlUrl">The URL used to view the ADR on GitHub.</param>
        /// <param name="content">The raw ADR markdown content.</param>
        public ArchitectureDecisionRecord(
            string applicationName,
            int number,
            string title,
            string lastUpdated,
            string downloadUrl,
            string htmlUrl,
            string? content)
        {
            ApplicationName = applicationName;
            Title = title;
            Number = number;
            LastUpdated = lastUpdated;
            DownloadUrl = downloadUrl;
            HtmlUrl = htmlUrl;

            if (content?.Length > 0)
            {
                Content = content;
            }
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="number">The ADR number.</param>
        /// <param name="title">The ADR title.</param>
        /// <param name="lastUpdated">The ADR last updated date.</param>
        /// <param name="downloadUrl">The ADR content url.</param>
        /// <param name="htmlUrl">The URL used to view the ADR on GitHub.</param>
        public ArchitectureDecisionRecord(
            string applicationName,
            int number,
            string title,
            string lastUpdated,
            string downloadUrl,
            string htmlUrl)
            : this(
                  applicationName,
                  number,
                  title,
                  lastUpdated,
                  downloadUrl,
                  htmlUrl,
                  content: null)
        { }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected ArchitectureDecisionRecord()
        { }

        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string? ApplicationName { get; protected set; }

        /// <summary>
        /// Gets the ADR title.
        /// </summary>
        public string? Title { get; protected set; }

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public int Number { get; protected set; }

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

        /// <summary>
        /// Gets the url used to view the ADR on GitHub.
        /// </summary>
        public string? HtmlUrl { get; protected set; }

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

        /// <summary>
        /// Updates the ADR content url.
        /// </summary>
        /// <param name="htmlUrl">The content url.</param>
        public void UpdateHtmlUrl(string htmlUrl)
        {
            if (string.IsNullOrWhiteSpace(htmlUrl))
            {
                throw new ArgumentException($"The {nameof(htmlUrl)} argument is required.", nameof(htmlUrl));
            }

            HtmlUrl = htmlUrl;
        }
    }
}
