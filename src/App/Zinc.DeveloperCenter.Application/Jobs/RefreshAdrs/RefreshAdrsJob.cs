using System;
using RedLine.Application.Jobs;

namespace Zinc.DeveloperCenter.Application.Jobs.RefreshAdrs
{
    /// <summary>
    /// A job used to refresh the ADRs in the database.
    /// </summary>
    public class RefreshAdrsJob : JobBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        public RefreshAdrsJob(string tenantId, Guid correlationId)
            : base(tenantId, correlationId)
        {
            TransactionIsolation = System.Transactions.IsolationLevel.ReadUncommitted;
            TransactionTimeout = TimeSpan.FromMinutes(30); // This can be a long running transaction
        }

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Refresh ADRs job";

        /// <inheritdoc/>
        public override string ActivityDescription => "Adds and updates ADRs in the application database using the GitHub api.";
    }
}
