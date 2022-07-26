using RedLine.Data.Repositories;
using Zinc.DeveloperCenter.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Model;

namespace Zinc.DeveloperCenter.Data.DataQueries
{
    /// <summary>
    /// Retrieves a list of most viewed <see cref="ArchitectureDecisionRecord"/>s.
    /// </summary>
    public class GetMostViewedArchitectureDecisionRecordsDataQuery : DbAggregateQueryBase<ArchitectureDecisionRecord>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="topN">The top N number of ADRs to return.</param>
        public GetMostViewedArchitectureDecisionRecordsDataQuery(string tenantId, int topN)
        {
            Command = ArchitectureDecisionRecordRepository.Sql.MostViewed;
            Params = new { tenantId, topN };
        }
    }
}
