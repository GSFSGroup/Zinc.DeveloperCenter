using System;
using RedLine.Application.Queries;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR
{
    /// <summary>
    /// Get product type query.
    /// </summary>
    public class UXGitHubGetAdrLastUpdatedDateQuery : QueryBase<DateTime>
    {
        /// <summary>
        /// Initializes the query.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="applicationName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <param name="adrTitle"> Path to ADR in GitHub repo.</param>
        public UXGitHubGetAdrLastUpdatedDateQuery(string tenantId, Guid correlationId, string applicationName, string adrTitle)
            : base(tenantId, correlationId)
        {
            this.ApplicationName = applicationName;
            this.AdrTitle = adrTitle;
        }

        /// <inheritdoc />
        public override string ActivityDisplayName => "Get last updated date.";

        /// <inheritdoc />
        public override string ActivityDescription => "Get the time of the most recent update of an ADR.";

        /// <summary>
        /// Full name of repo.
        /// </summary>
        public string ApplicationName { get; }

        /// <summary>
        /// Full title of Adr. ex: adr-0001-full-adr-name.md.
        /// </summary>
        public string AdrTitle { get; }
    }
}
