using System.Data;
using System.Threading.Tasks;
using Dapper;
using Zinc.DeveloperCenter.Domain.Services.ViewCounter;

namespace Zinc.DeveloperCenter.Application.Services.ViewCounter
{
    /// <inheritdoc/>
    public class ViewCounterService : IViewCounterService
    {
        private readonly IDbConnection connection;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        public ViewCounterService(IDbConnection connection)
        {
            this.connection = connection;
        }

        /// <inheritdoc/>
        public async Task<int> GetViewCount(string tenantId, string applicationName, string filePath)
        {
            return await connection.ExecuteScalarAsync<int>(
                Sql.GetViewCount,
                new { tenantId, applicationName, filePath }).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<int> UpdateViewCount(string tenantId, string applicationName, string filePath)
        {
            var result = await connection.ExecuteScalarAsync<int>(
                Sql.UpdateViewCount,
                new { tenantId, applicationName, filePath }).ConfigureAwait(false);

            if (result == 0)
            {
                result = await connection.ExecuteScalarAsync<int>(
                    Sql.InsertViewCount,
                    new { tenantId, applicationName, filePath }).ConfigureAwait(false);
            }

            return result;
        }

        private static class Sql
        {
            public static readonly string GetViewCount = @"
SELECT view_count
FROM developercenter.architecture_decision_record_viewcount
WHERE id = (
    SELECT adr.id
    FROM developercenter.architecture_decision_record AS adr
    WHERE adr.tenant_id = @tenantId
    AND adr.application_name = @applicationName
    AND adr.file_path = @filePath
);";

            public static readonly string InsertViewCount = @"
INSERT INTO developercenter.architecture_decision_record_viewcount (
    id,
    view_count
) VALUES (
    (
        SELECT adr.id
        FROM developercenter.architecture_decision_record AS adr
        WHERE adr.tenant_id = @tenantId
        AND adr.application_name = @applicationName
        AND adr.file_path = @filePath
    ),
    1
)
ON CONFLICT (id) DO
UPDATE SET view_count = view_count + 1
RETURNING view_count;";

            public static readonly string UpdateViewCount = @"
UPDATE developercenter.architecture_decision_record_viewcount
SET view_count = view_count + 1
WHERE id = (
    SELECT adr.id
    FROM developercenter.architecture_decision_record AS adr
    WHERE adr.tenant_id = @tenantId
    AND adr.application_name = @applicationName
    AND adr.file_path = @filePath
)
RETURNING view_count
;";
        }
    }
}
