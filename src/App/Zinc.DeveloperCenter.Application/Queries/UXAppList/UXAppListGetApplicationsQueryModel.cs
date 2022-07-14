using System;

namespace Zinc.DeveloperCenter.Application.Queries.UXAppList
{
    /// <summary>
    /// The model returned by the UXAppListGetApplicationsQuery.
    /// </summary>
    public class UXAppListGetApplicationsQueryModel
    {
        /// <summary>
        /// The application name.
        /// </summary>
        public string? ApplicationName { get; set; }

        /// <summary>
        /// The application display name.
        /// </summary>
        public string? ApplicationDisplayName { get; set; }

        /// <summary>
        /// The element name.
        /// </summary>
        public string? Element { get; set; }
    }
}
