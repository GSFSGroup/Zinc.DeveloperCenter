using RedLine.Data.Repositories;
using Zinc.DeveloperCenter.Domain.Model;

namespace Zinc.DeveloperCenter.Data.DataQueries
{
    /// <summary>
    /// A query used to retrieve ADRs for a given application.
    /// </summary>
    public class GetArchitectureDecisionRecordsForApplication : DbAggregateQueryBase<ArchitectureDecisionRecord>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        public GetArchitectureDecisionRecordsForApplication(string applicationName)
        {
            Command = "SELECT * FROM developercenter.architecture_decision_record WHERE application_name = @applicationName";
            Params = new { applicationName };
        }
    }
}
