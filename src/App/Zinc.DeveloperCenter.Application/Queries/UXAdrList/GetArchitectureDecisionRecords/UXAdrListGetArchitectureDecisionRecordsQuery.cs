using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrList.GetArchitectureDecisionRecords
{
    /// <summary>
    /// Query to retrieve the ADRs for a given application.
    /// </summary>
    public class UXAdrListGetArchitectureDecisionRecordsQuery : QueryBase<PageableResult<UXAdrListGetArchitectureDecisionRecordsQueryModel>>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="applicationName">The application name.</param>
        public UXAdrListGetArchitectureDecisionRecordsQuery(
            string tenantId,
            Guid correlationId,
            string applicationName)
            : base(tenantId, correlationId)
        {
            ApplicationName = applicationName;
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName { get; init; }

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Get the ADRs for an application";

        /// <inheritdoc/>
        public override string ActivityDescription => "Retrieves the list of ADRs defined for an application.";
    }
}
