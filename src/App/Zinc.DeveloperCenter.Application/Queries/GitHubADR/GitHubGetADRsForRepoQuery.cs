using System;
using RedLine.Application.Queries;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR
{
    /// <summary>
    /// Get product type query.
    /// </summary>
    public class GitHubGetADRsForRepoQuery : QueryBase<GitHubAdrModel>
    {
        /// <summary>
        /// Initializes the query.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="repoDotName">The product type id.</param>
        public GitHubGetADRsForRepoQuery(string tenantId, Guid correlationId, string repoDotName)
            : base(tenantId, correlationId)
        {
            this.RepoDotName = repoDotName;
        }

        /// <inheritdoc />
        public override string ActivityDisplayName => "Get an ADR.";

        /// <inheritdoc />
        public override string ActivityDescription => "Gets the list of ADRs in a GSFS GitHub group repo.";

        /// <summary>
        /// The unique key for the product.
        /// </summary>
        public string RepoDotName { get; }
    }
}
