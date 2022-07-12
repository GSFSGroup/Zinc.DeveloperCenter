using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;

namespace Zinc.DeveloperCenter.Application.Queries.UXGetADRsForAppliaction
{
    /// <summary>
    /// A query used to retrieve the list of ADRs defined in a given application.
    /// </summary>
    public class UXGetADRsForAppliactionQuery : QueryBase<PageableResult<UXGetADRsForAppliactionQueryModel>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UXGetADRsForAppliactionQuery"/> class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="applicationName">The application name.</param>
        public UXGetADRsForAppliactionQuery(string tenantId, Guid correlationId, string applicationName)
            : base(tenantId, correlationId)
        {
            ApplicationName = applicationName;
        }

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Get ADRs for an application";

        /// <inheritdoc/>
        public override string ActivityDescription => "Retrieves the list of ADRs defined in a given application.";

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName { get; init; }
    }
}
