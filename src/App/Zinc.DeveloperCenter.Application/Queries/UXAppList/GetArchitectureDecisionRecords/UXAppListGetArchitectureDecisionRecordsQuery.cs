using System;
using System.Collections.Generic;
using RedLine.Application.Queries;

namespace Zinc.DeveloperCenter.Application.Queries.UXAppList.GetArchitectureDecisionRecords
{
    /// <summary>
    /// A query to retrieve the ADRs for a given application.
    /// </summary>
    public class UXAppListGetArchitectureDecisionRecordsQuery : QueryBase<IEnumerable<UXAppListGetArchitectureDecisionRecordsQueryModel>>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="applicationName">The application name.</param>
        public UXAppListGetArchitectureDecisionRecordsQuery(
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
    }
}
