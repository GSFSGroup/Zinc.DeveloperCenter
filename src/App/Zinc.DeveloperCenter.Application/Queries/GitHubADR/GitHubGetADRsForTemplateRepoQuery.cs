using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR
{
    /// <summary>
    /// Get product type query.
    /// </summary>
    public class GitHubGetADRsForTemplateRepoQuery : QueryBase<PageableResult<GitHubAdrSummaryModel>>
    {
        /// <summary>
        /// Initializes the query.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        public GitHubGetADRsForTemplateRepoQuery(string tenantId, Guid correlationId, int pageNumber, int pageSize)
            : base(tenantId, correlationId)
        {
        }

        /// <inheritdoc />
        public override string ActivityDisplayName => "List the global, RedLine ADRs for the Zinc.Templates repo in the GSFS group.";

        /// <inheritdoc />
        public override string ActivityDescription => "List the global, RedLine ADRs for the Zinc.Templates repo in the GSFS group.";

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
