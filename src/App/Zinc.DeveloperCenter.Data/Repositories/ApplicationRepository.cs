using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RedLine.Data.Outbox;
using RedLine.Data.Repositories;
using RedLine.Domain;
using RedLine.Domain.Model;
using RedLine.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Model;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Data.Repositories
{
    /// <summary>
    /// Provides an implementation of <see cref="IApplicationRepository"/>.
    /// </summary>
    public class ApplicationRepository : DbRepositoryBase<Application>, IApplicationRepository
    {
        private readonly IDbConnection connection;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The activity context.</param>
        /// <param name="outbox">The outbox.</param>
        public ApplicationRepository(IActivityContext context, IOutbox outbox)
            : base(context, outbox)
        {
            this.connection = context.Connection();
        }

        /// <inheritdoc/>
        protected override async Task<bool> ExistsInternal(string key)
        {
            var keyParts = key.Split('/');
            var tenantId = keyParts[0];
            var applicationName = keyParts[1];

            var args = new { TenantId = tenantId, ApplicationName = applicationName };

            return await connection.ExecuteScalarAsync<bool>(Sql.Exists, args).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override async Task<Application> ReadInternal(string key)
        {
            var keyParts = key.Split('/');
            var tenantId = keyParts[0];
            var applicationName = keyParts[1];

            var args = new { TenantId = tenantId, ApplicationName = applicationName };

            return (await connection.QueryAsync<Application>(Sql.Read, args).ConfigureAwait(false))
                .SingleOrDefault()!;
        }

        /// <inheritdoc/>
        protected override async Task<Application> ReadInternal(IDbAggregateQuery<Application> qry)
        {
            return (await connection.QueryAsync<Application>(qry.Command, qry.Params).ConfigureAwait(false))
                .SingleOrDefault()!;
        }

        /// <inheritdoc/>
        protected override async Task<PageableResult<Application>> QueryInternal(IDbAggregateQuery<Application> qry)
        {
            var results = await connection.QueryAsync<Application>(qry.Command, qry.Params).ConfigureAwait(false);

            return new PageableResult<Application>(results);
        }

        /// <inheritdoc/>
        protected override async Task<int> SaveInternal(Application aggregate, string etag, string newETag)
        {
            var args = new
            {
                TenantId = aggregate.TenantId,
                ApplicationName = aggregate.ApplicationName,
                ApplicationDisplayName = aggregate.ApplicationDisplayName,
                ApplicationElement = aggregate.ApplicationElement,
            };

            return await connection.ExecuteAsync(Sql.Save, args).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "By design.")]
        internal static class Sql
        {
            private static readonly string TableName = "developercenter.application";

            internal static readonly string Exists = $@"
SELECT EXISTS (
    SELECT 1
    FROM {TableName}
    WHERE tenant_id = @TenantId
    AND application_name = @ApplicationName
);
";

            internal static readonly string Read = $@"
SELECT *
FROM {TableName}
WHERE tenant_id = @TenantId
AND application_name = @ApplicationName
;";

            internal static readonly string ReadAll = $@"
SELECT *
FROM {TableName}
WHERE tenant_id = @TenantId
;";

            internal static readonly string Save = $@"
INSERT INTO {TableName} (
    tenant_id,
    application_name,
    application_display_name,
    application_element
) VALUES (
    @TenantId,
    @ApplicationName,
    @ApplicationDisplayName,
    @ApplicationElement
)
ON CONFLICT ON CONSTRAINT {TableName}_key
DO UPDATE SET
    application_display_name = @ApplicationDisplayName,
    application_element = @ApplicationElement
;    
";
        }
    }
}