using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.A3.Authentication;
using RedLine.Domain;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.UXFavorites;

namespace Zinc.DeveloperCenter.Host.Web.Controllers.V1
{
    /// <summary>
    /// An api that provides endpoints for adding and removing ADR favorites.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("ux/v{version:apiVersion}/{tenantId}/architecture-decision-records/favorites")]
    public class UXFavoritesController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly IAuthenticationToken user;
        private readonly ILogger<UXFavoritesController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">Query mediator to interpret protocols.</param>
        /// <param name="correlationId">Unique ID for each request.</param>
        /// <param name="tenantId">Identifier for tenant.</param>
        /// <param name="user">The current user.</param>
        /// <param name="logger">Diagnostic logger.</param>
        public UXFavoritesController(
            IMediator mediator,
            ICorrelationId correlationId,
            ITenantId tenantId,
            IAuthenticationToken user,
            ILogger<UXFavoritesController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.user = user;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the list of favorite ADRs for the current user.
        /// </summary>
        /// <returns>The collection of favorite architecture decision records for the current user.</returns>
        /// <response code="200">The request was successful.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(PageableResult<UXGetFavoritesQueryModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            return await this.Execute(logger, async () =>
            {
                var request = new UXGetFavoritesQuery(
                    tenantId.Value,
                    correlationId.Value,
                    user.UserId);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }
    }
}
