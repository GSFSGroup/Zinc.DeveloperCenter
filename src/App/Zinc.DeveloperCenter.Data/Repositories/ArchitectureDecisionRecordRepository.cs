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
    /// Privides an implementation of IArchitectureDecisionRecordRepository.
    /// </summary>
    public class ArchitectureDecisionRecordRepository : DbRepositoryBase<ArchitectureDecisionRecord>, IArchitectureDecisionRecordRepository
    {
        private readonly IDbConnection connection;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="context">The activity context.</param>
        /// <param name="outbox">The outbox.</param>
        public ArchitectureDecisionRecordRepository(IActivityContext context, IOutbox outbox)
            : base(context, outbox)
        {
            this.connection = context.Connection();
        }

        /// <inheritdoc/>
        protected override async Task<bool> ExistsInternal(string key)
        {
            var keyParts = key.Split('/', 3, System.StringSplitOptions.RemoveEmptyEntries);
            var tenantId = keyParts[0];
            var applicationName = keyParts[1];
            var filePath = keyParts[2];

            var args = new
            {
                tenantId,
                applicationName,
                filePath,
            };

            return await connection.ExecuteScalarAsync<bool>(Sql.Exists, args).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override async Task<PageableResult<ArchitectureDecisionRecord>> QueryInternal(IDbAggregateQuery<ArchitectureDecisionRecord> qry)
        {
            var results = await connection.QueryAsync<ArchitectureDecisionRecord>(qry.Command, qry.Params).ConfigureAwait(false);

            return new PageableResult<ArchitectureDecisionRecord>(results);
        }

        /// <inheritdoc/>
        protected override async Task<ArchitectureDecisionRecord> ReadInternal(string key)
        {
            var keyParts = key.Split('/', 3, System.StringSplitOptions.RemoveEmptyEntries);
            var tenantId = keyParts[0];
            var applicationName = keyParts[1];
            var filePath = keyParts[2];

            var args = new
            {
                tenantId,
                applicationName,
                filePath,
            };

            return (await connection.QueryAsync<ArchitectureDecisionRecord>(Sql.Read, args)
                .ConfigureAwait(false))
                .SingleOrDefault()!;
        }

        /// <inheritdoc/>
        protected override async Task<ArchitectureDecisionRecord> ReadInternal(IDbAggregateQuery<ArchitectureDecisionRecord> qry)
        {
            return (await connection.QueryAsync<ArchitectureDecisionRecord>(qry.Command, qry.Params)
                .ConfigureAwait(false))
                .SingleOrDefault()!;
        }

        /// <inheritdoc/>
        protected override async Task<int> SaveInternal(ArchitectureDecisionRecord aggregate, string etag, string newETag)
        {
            var args = new
            {
                aggregate.TenantId,
                aggregate.ApplicationName,
                aggregate.FilePath,
                aggregate.LastUpdatedBy,
                aggregate.LastUpdatedOn,
            };

            var id = await connection.ExecuteScalarAsync<int>(Sql.Save, args).ConfigureAwait(false);

            if (aggregate.Content?.Length > 0)
            {
                await connection.ExecuteAsync(
                    Sql.SaveSearchVector,
                    new { Id = id, Content = aggregate.Content }).ConfigureAwait(false);
            }

            return 1;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "By design.")]
        internal static class Sql
        {
            private static readonly string TableName = "developercenter.architecture_decision_record";

            internal static readonly string Exists = $@"
SELECT EXISTS (
    SELECT 1 FROM {TableName}
    WHERE tenant_id = @tenantId
    AND   application_name = @applicationName
    AND   file_path = @filePath
);";

            internal static readonly string MostViewed = $@"
SELECT adr.*, COALESCE(views.view_count, 0) AS total_views
FROM {TableName} AS adr
LEFT OUTER JOIN {TableName}_viewcount AS views
    ON views.id = adr.id
WHERE adr.tenant_id = @tenantId
AND COALESCE(views.view_count, 0) > 0
ORDER BY COALESCE(views.view_count, 0) DESC
LIMIT @topN
;";

            internal static readonly string MyFavorites = $@"
SELECT adr.*, COALESCE(views.view_count, 0) AS total_views
FROM {TableName} AS adr
INNER JOIN {TableName}_favorite AS favs
    ON favs.{TableName}_id = adr.id AND favs.user_id = @userId
LEFT OUTER JOIN {TableName}_viewcount AS views
    ON views.id = adr.id
WHERE adr.tenant_id = @tenantId
;";

            internal static readonly string Read = $@"
SELECT adr.*, COALESCE(views.view_count, 0) AS total_views
FROM {TableName} AS adr
LEFT OUTER JOIN {TableName}_viewcount AS views
    ON views.id = adr.id
WHERE adr.tenant_id = @tenantId
AND   adr.application_name = @applicationName
AND   adr.file_path = @filePath
;";

            internal static readonly string ReadAllForApplication = $@"
SELECT adr.*, COALESCE(views.view_count, 0) AS total_views
FROM {TableName} AS adr
LEFT OUTER JOIN {TableName}_viewcount AS views
    ON views.id = adr.id
WHERE tenant_id = @tenantId
AND   application_name = @applicationName
;";

            internal static readonly string Save = $@"
INSERT INTO {TableName} (
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on
) VALUES (
    @TenantId,
    @ApplicationName,
    @FilePath,
    @LastUpdatedBy,
    @LastUpdatedOn
)
ON CONFLICT (tenant_id, application_name, file_path)
DO UPDATE SET
    last_updated_by = EXCLUDED.last_updated_by,
    last_updated_on = EXCLUDED.last_updated_on
RETURNING id
;";

            internal static readonly string SaveSearchVector = $@"
INSERT INTO {TableName}_search (
    id,
    search_vector
) VALUES (
    @Id,
    to_tsvector('english', @Content)
)
ON CONFLICT (id)
DO UPDATE SET
    search_vector = to_tsvector('english', @Content)
;";

            /* The current search implementation is rather simple, but Postgres allows for much more
             * complicated searches, including phrase searches and fuzzy searches. The following post
             * has a good overview of Postgres full text searching:
             * https://hevodata.com/blog/postgresql-full-text-search-setup/
             * */
            internal static readonly string SearchArchitectureDecisionRecords = $@"
SELECT adr.*, COALESCE(views.view_count, 0) AS total_views
FROM {TableName} AS adr
INNER JOIN {TableName}_search AS search
    ON search.id = adr.id AND adr.tenant_id = @tenantId
LEFT OUTER JOIN {TableName}_viewcount AS views
    ON views.id = adr.id
WHERE search.search_vector @@ to_tsquery('english', @searchPattern)
;";
        }
    }
}
