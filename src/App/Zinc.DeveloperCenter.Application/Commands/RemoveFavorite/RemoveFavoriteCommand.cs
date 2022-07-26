using System;
using MediatR;
using RedLine.Application.Commands;

namespace Zinc.DeveloperCenter.Application.Commands.RemoveFavorite
{
    /// <summary>
    /// A command used to remove a favorite ADR.
    /// </summary>
    public class RemoveFavoriteCommand : CommandBase<Unit>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="filePath">The ADR file path in GitHub.</param>
        public RemoveFavorite(
            string tenantId,
            Guid correlationId,
            string applicationName,
            string filePath)
            : base(tenantId, correlationId)
        {
            ApplicationName = applicationName;
            FilePath = filePath;
        }

        /// <summary>
        /// Gets the application name.
        /// </summary>
        public string ApplicationName { get; init; }

        /// <summary>
        /// Gets the GitHub file path to the ADR.
        /// </summary>
        public string FilePath { get; init; }

        /// <inheritdoc/>
        public override string ActivityDisplayName => "Remove a favorite ADR";

        /// <inheritdoc/>
        public override string ActivityDescription => "Removes a favorite ADR for the current user.";
    }
}
