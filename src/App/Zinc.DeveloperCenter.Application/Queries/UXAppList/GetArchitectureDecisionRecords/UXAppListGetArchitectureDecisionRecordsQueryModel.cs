using System;

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
        /// Gets the ADR title.
        /// </summary>
        public string? TitleDisplay { get; set; }

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public string? NumberDisplay { get; set; }

        /// <summary>
        /// Gets who last updated the ADR.
        /// </summary>
        public string? LastUpdatedBy { get; set; }

        /// <summary>
        /// Gets the ADR last updated date.
        /// </summary>
        public DateTimeOffset? LastUpdatedOn { get; set; }
    }
}
