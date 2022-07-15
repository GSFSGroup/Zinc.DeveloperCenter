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
        /// <param name="sortedOn"> One of three options to sort Adr list: last updated date, number, title.</param>
        /// <param name="sortAsc"> Denotes whether to sort ascending or descending.</param>
        public UXGitHubGetADRsForRepoQuery(string tenantId, Guid correlationId, string repoDotName, string sortedOn, bool sortAsc)
            : base(tenantId, correlationId)
        {
            this.RepoDotName = repoDotName;
            this.SortedOn = sortedOn;
            this.SortAsc = sortAsc;
        }

        /// <inheritdoc />
        public override string ActivityDisplayName => "Get an ADR";

        /// <inheritdoc />
        public override string ActivityDescription => "List the ADRs for a specific repo in the GSFS group.";

        /// <summary>
        /// The page number.
        /// </summary>
        public string RepoDotName { get; }

        /// <summary>
        /// The variable to sort on.
        /// </summary>
        public string SortedOn { get; }

        /// <summary>
        /// The direction to sort.
        /// </summary>
        public bool SortAsc { get; }
    }
}
