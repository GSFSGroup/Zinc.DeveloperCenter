using System.Threading.Tasks;
using Dapper;
using RedLine.Domain;
using Zinc.DeveloperCenter.Domain.Services.Favorites;

namespace Zinc.DeveloperCenter.Application.Services.Favorites
{
    /// <inheritdoc/>
    public class FavoritesService : IFavoritesService
    {
        private readonly IActivityContext context;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The current <see cref="IActivityContext"/>.</param>
        public FavoritesService(IActivityContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task AddFavorite(string tenantId, string applicationName, string filePath)
        {
            await context.Connection().ExecuteAsync(
                Sql.AddFavorite,
                new
                {
                    tenantId = tenantId,
                    applicationName = applicationName,
                    filePath = filePath,
                    userId = context.AccessToken().UserId,
                }).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task RemoveFavorite(string tenantId, string applicationName, string filePath)
        {
            await context.Connection().ExecuteAsync(
                Sql.RemoveFavorite,
                new
                {
                    tenantId = tenantId,
                    applicationName = applicationName,
                    filePath = filePath,
                    userId = context.AccessToken().UserId,
                }).ConfigureAwait(false);
        }

        private static class Sql
        {
            public static readonly string AddFavorite = @"
INSERT INTO developercenter.architecture_decision_record_favorite (
    architecture_decision_record_id,
    user_id
) VALUES (
    architecture_decision_record_id = (
        SELECT id
        FROM developercenter.architecture_decision_record
        WHERE tenant_id = @tenantId
        AND application_name = @applicationName
        AND file_path = @filePath
    ),
    @userId
)
ON CONFLICT (architecture_decision_record_id) DO NOTHING;
";

            public static readonly string RemoveFavorite = @"
DELETE FROM developercenter.architecture_decision_record_favorite
WHERE
    architecture_decision_record_id = (
        SELECT id
        FROM developercenter.architecture_decision_record
        WHERE tenant_id = @tenantId
        AND application_name = @applicationName
        AND file_path = @filePath
    )
    AND user_id = @userId;
";
        }
    }
}
