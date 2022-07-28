using System.Threading.Tasks;
using Dapper;
using RedLine.Domain;
using Zinc.DeveloperCenter.Domain.Services.MostViewed;

namespace Zinc.DeveloperCenter.Application.Services.MostViewed
{
    /// <inheritdoc/>
    public class MostViewedService : IMostViewedService
    {
        private readonly IActivityContext context;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The activity context for the current request.</param>
        public MostViewedService(IActivityContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<int> GetViewCount(string applicationName, string filePath)
        {
            return await context.Connection().ExecuteScalarAsync<int>(
                Sql.GetViewCount,
                new { tenantId = context.TenantId(), applicationName = applicationName, filePath = filePath }).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<int> UpdateViewCount(string applicationName, string filePath)
        {
            var result = await context.Connection().ExecuteScalarAsync<int>(
                Sql.UpdateViewCount,
                new { tenantId = context.TenantId(), applicationName = applicationName, filePath = filePath }).ConfigureAwait(false);

            if (result == 0)
            {
                result = await context.Connection().ExecuteScalarAsync<int>(
                    Sql.InsertViewCount,
                    new { tenantId = context.TenantId(), applicationName = applicationName, filePath = filePath }).ConfigureAwait(false);
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
UPDATE SET view_count = developercenter.architecture_decision_record_viewcount.view_count + 1
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
