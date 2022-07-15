namespace Zinc.DeveloperCenter.Application.Queries.UXAppList.GetApplications
{
    /// <summary>
    /// The model returned by the UXAppListGetApplicationsQuery.
    /// </summary>
    public class UXAppListGetApplicationsQueryModel
    {
        /// <summary>
        /// The application element name.
        /// </summary>
        public string? ApplicationElement { get; set; }

        /// <summary>
        /// The application name.
        /// </summary>
        public string? ApplicationName { get; set; }

        /// <summary>
        /// The application display name.
        /// </summary>
        public string? ApplicationDisplayName { get; set; }
    }
}
