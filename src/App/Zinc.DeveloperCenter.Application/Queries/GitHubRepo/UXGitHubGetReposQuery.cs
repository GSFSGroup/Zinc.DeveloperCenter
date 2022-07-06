using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.GitHubRepo.Models;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubRepo
{
    /// <summary>
    /// Lists the repos in the GSFS GitHub group.
    /// </summary>
    public class UXGitHubGetReposQuery : QueryBase<PageableResult<GitHubRepoModel>>
    {
        /// <summary>
        /// Intializes the query.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        public UXGitHubGetReposQuery(string tenantId, Guid correlationId, int pageNumber, int pageSize)
            : base(tenantId, correlationId)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }

        /// <inheritdoc />
        public override string ActivityDisplayName => "List the repos in the GSFS GitHub group.";

        /// <inheritdoc />
        public override string ActivityDescription => "Lists the repos in the GSFS GitHub group.";

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
