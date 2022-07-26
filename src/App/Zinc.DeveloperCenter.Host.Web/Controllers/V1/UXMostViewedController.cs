using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.Domain;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.UXMostUsed.GetMostViewed;

namespace Zinc.DeveloperCenter.Host.Web.Controllers.V1
{
    /// <summary>
    /// An api that provides endpoints for retrieving the most viewed architecture decision records.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("ux/v{version:apiVersion}/{tenantId}/architecture-decision-records/most-viewed")]
    public class UXMostViewedController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly ILogger<UXMostViewedController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">Query mediator to interpret protocols.</param>
        /// <param name="correlationId">Unique ID for each request.</param>
        /// <param name="tenantId">Identifier for tenant.</param>
        /// <param name="logger">Diagnostic logger.</param>
        public UXMostViewedController(
            IMediator mediator,
            ICorrelationId correlationId,
            ITenantId tenantId,
            ILogger<UXMostViewedController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the list of most viewed ADRs.
        /// </summary>
        /// <param name="top">The top N number of most viewed ADRs to return.</param>
        /// <returns>The collection of architecture decision records for the application (repository).</returns>
        /// <response code="200">The request was successful.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(PageableResult<UXGetMostViewedQueryModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Critical Code Smell", "S2360:Optional parameters should not be used", Justification = "By design.")]
        public async Task<IActionResult> GetMostViewed([FromQuery] int top = 6)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new UXGetMostViewedQuery(
                    tenantId.Value,
                    correlationId.Value,
                    top);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }
    }
}
