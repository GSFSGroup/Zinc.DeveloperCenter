using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR
{
    /// <summary>
    /// Get product type query.
    /// </summary>
    public class GitHubGetADRsForRepoQuery : QueryBase<PageableResult<GitHubAdrSummaryModel>>
    {
        /// <summary>
        /// Initializes the query.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="repoDotName">The product type id.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        public GitHubGetADRsForRepoQuery(string tenantId, Guid correlationId, string repoDotName, int pageNumber, int pageSize)
            : base(tenantId, correlationId)
        {
            this.RepoDotName = repoDotName;
        }

        /// <inheritdoc />
        public override string ActivityDisplayName => "List the ADRs for a specific repo in the GSFS group.";

        /// <inheritdoc />
        public override string ActivityDescription => "List the ADRs for a specific repo in the GSFS group.";

        /// <summary>
        /// The full repo name for the adr: ex. Platinum.Products.
        /// </summary>
        public string RepoDotName { get; }

        /// <summary>
        /// The page number.
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// The page size.
        /// </summary>
        public int PageSize { get; }
    }
}
