using System;
using RedLine.Application.Queries;
using RedLine.Domain.Model;

namespace Zinc.DeveloperCenter.Application.Queries.UXFavorites
{
    /// <summary>
    /// A query used to get a user's favorite ADRs.
    /// </summary>
    public class UXGetFavoritesQuery : QueryBase<PageableResult<UXGetFavoritesQueryModel>>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="userId">The user's id.</param>
        public UXGetFavoritesQuery(string tenantId, Guid correlationId, string userId)
            : base(tenantId, correlationId)
        {
            UserId = userId;
        }

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Get a user's favorite ADRs";

        /// <inheritdoc/>
        public override string ActivityDescription => "Retrieves the list of favorite ADRs for a user.";

        /// <summary>
        /// The user id who's favorites are being searched.
        /// </summary>
        public string UserId { get; }
    }
}
