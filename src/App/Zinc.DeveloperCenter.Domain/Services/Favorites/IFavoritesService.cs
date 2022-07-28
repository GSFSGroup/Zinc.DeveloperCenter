using System.Threading.Tasks;

namespace Zinc.DeveloperCenter.Domain.Services.Favorites
{
    /// <summary>
    /// A service used to manage user favorites.
    /// </summary>
    public interface IFavoritesService
    {
        /// <summary>
        /// Adds a favorite ADR for a user.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="filePath">The ADR file path.</param>
        /// <returns>Task.</returns>
        Task AddFavorite(string tenantId, string applicationName, string filePath);

        /// <summary>
        /// Removes a favorite ADR for a user.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="filePath">The ADR file path.</param>
        /// <returns>Task.</returns>
        Task RemoveFavorite(string tenantId, string applicationName, string filePath);
    }
}
