namespace Zinc.DeveloperCenter.Application.Queries.UXAppList.GetArchitectureDecisionRecords
{
    /// <summary>
    /// A model for the <see cref="UXAppListGetArchitectureDecisionRecordsQuery"/>.
    /// </summary>
    public class UXAppListGetArchitectureDecisionRecordsQueryModel
    {
        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string? ApplicationName { get; set; }

        /// <summary>
        /// Gets the ADR title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// Gets the ADR last updated date.
        /// </summary>
        public string? LastUpdated { get; set; }

        /// <summary>
        /// Gets the ADR content url.
        /// </summary>
        public string? ContentUrl { get; set; }
    }
}
