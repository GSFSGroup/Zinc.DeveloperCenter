using System;
using RedLine.Application.Jobs;

namespace Zinc.DeveloperCenter.Application.Jobs.RefreshAdrsLastUpdated
{
    /// <summary>
    /// A job that refreshes last updated details for ADRs.
    /// </summary>
    public class RefreshAdrsLastUpdatedJob : JobBase
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        public RefreshAdrsLastUpdatedJob(string tenantId, Guid correlationId)
            : base(tenantId, correlationId)
        {
            TransactionIsolation = System.Transactions.IsolationLevel.ReadUncommitted;
            TransactionTimeout = TimeSpan.FromMinutes(30); // This can be a long running transaction
        }

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Update ADR last updated details";

        /// <inheritdoc/>
        public override string ActivityDescription => "Updates the last updated details of ADRs in the local database from GitHub.";
    }
}
