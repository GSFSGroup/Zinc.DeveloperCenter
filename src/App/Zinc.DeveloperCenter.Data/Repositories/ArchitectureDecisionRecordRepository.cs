using System;
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
            var keyParts = key.Split('/');
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
            var keyParts = key.Split('/');
            var tenantId = keyParts[0];
            var applicationName = keyParts[1];
            var filePath = keyParts[2];

            var args = new
            {
                tenantId,
                applicationName,
                filePath,
            };

            var result = (await connection.QueryAsync<ArchitectureDecisionRecord>(Sql.Read, args)
                .ConfigureAwait(false))
                .SingleOrDefault();

            return result!;
        }

        /// <inheritdoc/>
        protected override async Task<ArchitectureDecisionRecord> ReadInternal(IDbAggregateQuery<ArchitectureDecisionRecord> qry)
        {
            return (await connection.QueryAsync<ArchitectureDecisionRecord>(qry.Command, qry.Params).ConfigureAwait(false))
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

            var id = await connection.ExecuteScalarAsync<Guid>(Sql.Save, args).ConfigureAwait(false);

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

            internal static readonly string Read = $@"
SELECT *
FROM {TableName}
WHERE tenant_id = @tenantId
AND   application_name = @applicationName
AND   file_path = @filePath
;";

            internal static readonly string ReadAllForApplication = $@"
SELECT *
FROM {TableName}
WHERE tenant_id = @tenantId
AND   application_name = @applicationName
;";

            internal static readonly string Save = $@"
INSERT INTO {TableName} (
    tenant_id,
    application_name,
    file_path,
    last_updated_by,
    last_updated_on,
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
REURNING id
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
SELECT adr.*
FROM {TableName} AS adr
INNER JOIN {TableName}_search AS search
    ON search.id = adr.id AND adr.tenant_id = @tenantId
WHERE search.search_vector @@ to_tsquery('english', @searchPattern)
;";
        }
    }
}
