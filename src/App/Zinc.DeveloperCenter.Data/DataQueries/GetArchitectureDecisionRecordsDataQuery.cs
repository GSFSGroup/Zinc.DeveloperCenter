using RedLine.Data.Repositories;
using Zinc.DeveloperCenter.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Model;

namespace Zinc.DeveloperCenter.Data.DataQueries
{
    /// <summary>
    /// Retrieves a list of <see cref="ArchitectureDecisionRecord"/>s.
    /// </summary>
    public class GetArchitectureDecisionRecordsDataQuery : DbAggregateQueryBase<ArchitectureDecisionRecord>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        public GetArchitectureDecisionRecordsDataQuery(string applicationName)
        {
            Command = ArchitectureDecisionRecordRepository.Sql.ReadAllForApplication;
            Params = new { applicationName };
        }
    }
}
