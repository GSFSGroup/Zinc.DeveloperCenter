using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrSearch
{
    /// <summary>
    /// Query used to search for ADRs across all applications.
    /// </summary>
    public class UXSearchArchitectureDecisionRecordsQuery : QueryBase<PageableResult<UXSearchArchitectureDecisionRecordsQueryModel>>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="searchPattern">The search pattern.</param>
        public UXSearchArchitectureDecisionRecordsQuery(
            string tenantId,
            Guid correlationId,
            string searchPattern)
            : base(tenantId, correlationId)
        {
            SearchPattern = searchPattern;
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string SearchPattern { get; init; }

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Search for ADRs";

        /// <inheritdoc/>
        public override string ActivityDescription => "Performs a full-text search for ADRs across all applications.";
    }
}
