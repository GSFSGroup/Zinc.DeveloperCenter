using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.Domain;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.UXAppList.GetApplications;

namespace Zinc.DeveloperCenter.Host.Web.Controllers.V1
{
    /// <summary>
    /// An api that provides endpoints for retrieving GitHub application (repository) data.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("ux/v{version:apiVersion}/{tenantId}/applications")]
    public class UXAppListController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly ILogger<UXGitHubRepoController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">Query mediator to interpret protocols.</param>
        /// <param name="correlationId">Unique ID for each request.</param>
        /// <param name="tenantId">Identifier for tenant.</param>
        /// <param name="logger">Diagnostic logger.</param>
        public UXAppListController(
            IMediator mediator,
            ICorrelationId correlationId,
            ITenantId tenantId,
            ILogger<UXAppListController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the list of applications (repositories) in a GitHub organization.
        /// </summary>
        /// <returns>The collection of applications defined in GitHub.</returns>
        /// <response code="200">The request was successful.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(PageableResult<UXAppListGetApplicationsQueryModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            return await this.Execute(logger, async () =>
            {
                var request = new UXAppListGetApplicationsQuery(
                    tenantId.Value,
                    correlationId.Value);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }
    }
}
