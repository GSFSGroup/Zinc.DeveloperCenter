using System;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrFavorites.GetMostViewed
{
    /// <summary>
    /// A model for the <see cref="UXGetMostViewedQuery"/>.
    /// </summary>
    public class UXGetMostViewedQueryModel
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
        /// Gets the path to the ADR in the repository.
        /// </summary>
        public string? FilePath { get; set; }

        /// <summary>
        /// Gets who last updated the ADR.
        /// </summary>
        public string? LastUpdatedBy { get; set; }

        /// <summary>
        /// Gets the ADR last updated date.
        /// </summary>
        public DateTimeOffset? LastUpdatedOn { get; set; }

        /// <summary>
        /// Gets the total views for the ADR.
        /// </summary>
        public int TotalViews { get; set; }
    }
}
