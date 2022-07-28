using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.Domain;
using Zinc.DeveloperCenter.Application.Commands.AddFavorite;
using Zinc.DeveloperCenter.Application.Commands.RemoveFavorite;
using Zinc.DeveloperCenter.Host.Web.Models;

namespace Zinc.DeveloperCenter.Host.Web.Controllers.V1
{
    /// <summary>
    /// An api used to manage user favorites.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/{tenantId}/architecture-decision-records/favorites")]
    public class FavoritesController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly ILogger<FavoritesController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">Query mediator to interpret protocols.</param>
        /// <param name="correlationId">Unique ID for each request.</param>
        /// <param name="tenantId">Identifier for tenant.</param>
        /// <param name="logger">Diagnostic logger.</param>
        public FavoritesController(
            IMediator mediator,
            ICorrelationId correlationId,
            ITenantId tenantId,
            ILogger<FavoritesController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.logger = logger;
        }

        /// <summary>
        /// Adds a favorite ADR for user.
        /// </summary>
        /// <param name="model">A model containing the ADR details.</param>
        /// <response code="204">The request was successful.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        /// <returns>No content.</returns>
        [HttpPost]
        public async Task<IActionResult> AddFavorite(AddFavoriteModel model)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new AddFavoriteCommand(
                    tenantId.Value,
                    correlationId.Value,
                    model.ApplicationName,
                    model.FilePath);

                await mediator.Send(request).ConfigureAwait(false);

                return NoContent();
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes a favorite ADR for user.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="path">The GitHub ADR path.</param>
        /// <response code="204">The request was successful.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        /// <returns>No content.</returns>
        [HttpDelete("{applicationName}")]
        public async Task<IActionResult> RemoveFavorite(string applicationName, [FromQuery] string path)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new RemoveFavoriteCommand(
                    tenantId.Value,
                    correlationId.Value,
                    applicationName,
                    path);

                await mediator.Send(request).ConfigureAwait(false);

                return NoContent();
            }).ConfigureAwait(false);
        }
    }
}
