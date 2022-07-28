using RedLine.Data.Repositories;
using Zinc.DeveloperCenter.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Model;

namespace Zinc.DeveloperCenter.Data.DataQueries
{
    /// <summary>
    /// Retrieves a list of favorite <see cref="ArchitectureDecisionRecord"/>s for a user.
    /// </summary>
    public class GetFavoriteArchitectureDecisionRecordsDataQuery : DbAggregateQueryBase<ArchitectureDecisionRecord>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="userId">The application name.</param>
        public GetFavoriteArchitectureDecisionRecordsDataQuery(string tenantId, string userId)
        {
            Command = ArchitectureDecisionRecordRepository.Sql.MyFavorites;
            Params = new { tenantId, userId };
        }
    }
}
