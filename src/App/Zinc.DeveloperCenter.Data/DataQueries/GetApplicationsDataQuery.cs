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
        public GetApplicationsDataQuery()
        {
            Command = ApplicationRepository.Sql.ReadAll;
        }
    }
}
