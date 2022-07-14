using RedLine.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Model;

namespace Zinc.DeveloperCenter.Data.DataQueries
{
    /// <summary>
    /// Retrieves a list of <see cref="ArchitectureDecisionRecord"/>s.
    /// </summary>
    public class UXAppListGetArchitectureDecisionRecordsDataQuery : DbAggregateQueryBase<ArchitectureDecisionRecord>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        public UXAppListGetArchitectureDecisionRecordsDataQuery(string applicationName)
        {
            Command = "SELECT * FROM developercenter.architecture_decision_record WHERE application_name = @applicationName";
            Params = new { applicationName };
        }
    }
}
