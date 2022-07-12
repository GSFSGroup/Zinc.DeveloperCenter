namespace Zinc.DeveloperCenter.Application.Queries.UXGetADRsForAppliaction
{
    /// <summary>
    /// A model used to return the ADRs for an application
    /// </summary>
    public class UXGetADRsForAppliactionQueryModel
    {
        /// <summary>
        /// Gets the application name where the ADR is defined.
        /// </summary>
        public string? ApplicationName { get; set; }

        /// <summary>
        /// Gets the application display name where the ADR is defined.
        /// </summary>
        public string? ApplicationDisplayName { get; set; }

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
        /// Gets the ADR content.
        /// </summary>
        public string? Content { get; set; }
    }
}
