namespace Zinc.DeveloperCenter.Host.Web.Models
{
    /// <summary>
    /// A model used to add a favorite.
    /// </summary>
    public class AddFavoriteModel
    {
        /// <summary>
        /// The application name.
        /// </summary>
        public string ApplicationName { get; set; } = string.Empty;

        /// <summary>
        /// The ADR GitHub file path.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
    }
}
