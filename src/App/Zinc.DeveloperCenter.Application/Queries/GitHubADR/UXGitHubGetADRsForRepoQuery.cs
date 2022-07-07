using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR
{
    /// <summary>
    /// Get product type query.
    /// </summary>
    public class UXGitHubGetADRsForRepoQuery : QueryBase<PageableResult<GitHubAdrSummaryModel>>
    {
        /// <summary>
        /// Initializes the query.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="repoDotName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        public UXGitHubGetADRsForRepoQuery(string tenantId, Guid correlationId, string repoDotName)
            : base(tenantId, correlationId)
        {
            this.RepoDotName = repoDotName;
        }

        /// <inheritdoc />
        public override string ActivityDisplayName => "Get an ADR";

        /// <inheritdoc />
        public override string ActivityDescription => "List the ADRs for a specific repo in the GSFS group.";

        /// <summary>
        /// The page number.
        /// </summary>
        public string RepoDotName { get; }
    }
}
