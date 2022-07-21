using RedLine.Data.Repositories;
using Zinc.DeveloperCenter.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Model;

namespace Zinc.DeveloperCenter.Data.DataQueries
{
    /// <summary>
    /// A query used to perform a full text search across ADRs.
    /// </summary>
    public class SearchArchitectureDecisionRecordsDataQuery : DbAggregateQueryBase<ArchitectureDecisionRecord>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="searchPattern">The search pattern.</param>
        public SearchArchitectureDecisionRecordsDataQuery(string tenantId, string searchPattern)
        {
            Command = ArchitectureDecisionRecordRepository.Sql.SearchArchitectureDecisionRecords;
            Params = new { tenantId, searchPattern };
        }
    }
}
