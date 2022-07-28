using System.Threading.Tasks;

namespace Zinc.DeveloperCenter.Domain.Services.MostViewed
{
    /// <summary>
    /// A service used to update an ADR view counter.
    /// </summary>
    public interface IMostViewedService
    {
        /// <summary>
        /// Gets the view count for an ADR.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="filePath">The ADR file path.</param>
        /// <returns>The total ADR views.</returns>
        Task<int> GetViewCount(string tenantId, string applicationName, string filePath);

        /// <summary>
        /// Updates the view counter for an ADR and returns the total views.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="filePath">The ADR file path.</param>
        /// <returns>The total ADR views.</returns>
        Task<int> UpdateViewCount(string tenantId, string applicationName, string filePath);
    }
}
