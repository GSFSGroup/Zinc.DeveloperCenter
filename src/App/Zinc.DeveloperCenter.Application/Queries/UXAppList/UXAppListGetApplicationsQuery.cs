using System;
using System.Collections.Generic;
using RedLine.Application.Queries;

namespace Zinc.DeveloperCenter.Application.Queries.UXAppList
{
    /// <summary>
    /// A query used to retrieve the list of applications.
    /// </summary>
    public class UXAppListGetApplicationsQuery : QueryBase<IEnumerable<UXAppListGetApplicationsQueryModel>>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        public UXAppListGetApplicationsQuery(string tenantId, Guid correlationId)
            : base(tenantId, correlationId)
        {
        }

        /// <inheritdoc/> 
        public override string ActivityDisplayName => "Gets a list of applications";

        /// <inheritdoc/> 
        public override string ActivityDescription => "Retrieves the list of applications for the AppList component.";
    }
}
