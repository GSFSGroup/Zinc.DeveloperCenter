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
        /// <param name="applicationDisplayName">The application display name.</param>
        /// <param name="title">The ADR title.</param>
        /// <param name="number">The ADR number.</param>
        /// <param name="lastUpdated">The ADR last updated date.</param>
        /// <param name="content">The ADR content.</param>
        public ArchitectureDecisionRecord(
            string applicationName,
            string applicationDisplayName,
            string title,
            string number,
            string lastUpdated,
            string content)
        {
            ApplicationName = applicationName;
            ApplicationDisplayName = applicationDisplayName;
            Title = title;
            Number = number;
            LastUpdated = lastUpdated;
            Content = content;
        }

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
        /// Gets the ADR content.
        /// </summary>
        public string? Content { get; protected set; }

        /// <inheritdoc/>
        public override string Key => $"{ApplicationName ?? string.Empty}/{Number ?? string.Empty}";
    }
}
