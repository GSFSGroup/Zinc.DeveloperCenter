using RedLine.Data.Repositories;
using Zinc.DeveloperCenter.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Model;

namespace Zinc.DeveloperCenter.Data.DataQueries
{
    /// <summary>
    /// Retrieves a list of <see cref="Application"/>s.
    /// </summary>
    public class GetApplicationsDataQuery : DbAggregateQueryBase<Application>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        public GetApplicationsDataQuery(string tenantId)
        {
            Command = ApplicationRepository.Sql.ReadAll;
            Params = new { tenantId };
        }
    }
}
