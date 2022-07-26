using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrFavorites.GetMostViewed
{
    /// <summary>
    /// A query used to return the top N most viewed ADRs.
    /// </summary>
    public class UXGetMostViewedQuery : QueryBase<PageableResult<UXGetMostViewedQueryModel>>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="topN">The top N count to return.</param>
        public UXGetMostViewedQuery(string tenantId, Guid correlationId, int topN)
            : base(tenantId, correlationId)
        {
            TopN = topN;
        }

        /// <inheritdoc/>
        public override string ActivityDisplayName => throw new NotImplementedException();

        /// <inheritdoc/>
        public override string ActivityDescription => throw new NotImplementedException();

        /// <summary>
        /// The top N to return (e.g. N = 10 to return the top 10).
        /// </summary>
        public int TopN { get; init; }
    }
}
