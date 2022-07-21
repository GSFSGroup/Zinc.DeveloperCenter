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
        public string? TitleDisplay => Title!.Replace('-', ' ');

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets the ADR number.
        /// </summary>
        public string NumberDisplay => string.Format($"adr-{0}", Number.ToString("0000"));

        /// <summary>
        /// Gets who last updated the ADR.
        /// </summary>
        public string? LastUpdatedBy { get; set; }

        /// <summary>
        /// Gets the ADR last updated date.
        /// </summary>
        public DateTime? LastUpdatedOn { get; set; }

        /// <summary>
        /// Gets the ADR download url.
        /// </summary>
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Gets the url used to view the ADR on GitHub.
        /// </summary>
        public string? HtmlUrl { get; set; }
    }
}
