using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using RedLine.Data.Exceptions;
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
        protected override async Task<ArchitectureDecisionRecord> ReadInternal(string key)
        {
            var components = key.Split('/');
            var applicationName = components[0];
            var number = components[1];

            var sql = @"
SELECT *
FROM developercenter.architecture_decision_record
WHERE application_name = @applicationName
AND   number = @number
";
            var args = new
            {
                applicationName,
                number,
            };

            var result = (await connection.QueryAsync<ArchitectureDecisionRecord>(sql, args).ConfigureAwait(false))
                .SingleOrDefault();

            return result ?? throw new ResourceNotFoundException(nameof(ArchitectureDecisionRecord), key);
        }

        /// <inheritdoc/>
        protected override async Task<PageableResult<ArchitectureDecisionRecord>> QueryInternal(IDbAggregateQuery<ArchitectureDecisionRecord> qry)
        {
            var results = await connection.QueryAsync<ArchitectureDecisionRecord>(qry.Command, qry.Params).ConfigureAwait(false);

            return new PageableResult<ArchitectureDecisionRecord>(results);
        }

        // TODO - Exists, Save
    }
}
