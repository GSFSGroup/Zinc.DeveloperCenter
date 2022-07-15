using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.Domain;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;

namespace Zinc.DeveloperCenter.Host.Web.Controllers.GitHub_API
{
    /// <summary>
    /// An api that provides endpoints for retrieving GitHub data.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("ux/v{version:apiVersion}/{tenantId}/adrs")]
    public class UXGitHubAdrController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly ILogger<UXGitHubAdrController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">Query mediator to interpret protocols.</param>
        /// <param name="correlationId">Unique ID for each request.</param>
        /// <param name="tenantId">Identifier for tenant.</param>
        /// <param name="logger">Diagnostic logger.</param>
        public UXGitHubAdrController(
            IMediator mediator,
            ICorrelationId correlationId,
            ITenantId tenantId,
            ILogger<UXGitHubAdrController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the list of ADRs in a GSFS GitHub group repo.
        /// </summary>
        /// <param name="repoDotName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <param name="adrTitle"> Full title of Adr. ex: adr-0001-full-adr-name.md.</param>
        /// <returns>The collection of grantable activities for the user.</returns>
        /// <response code="200">The grantable activities were returned.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(GitHubAdrSummaryModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet("update-dates/{repoDotName}/{adrTitle}")]
        public async Task<IActionResult> GitHubUdpateLastUpdatedDates(string repoDotName, string adrTitle)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new UXGitHubGetAdrLastUpdatedDateQuery(
                    tenantId.Value,
                    correlationId.Value,
                    repoDotName,
                    adrTitle);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the list of ADRs in a GSFS GitHub group repo.
        /// </summary>
        /// <param name="repoDotName"> Full name of repo for Adr. ex: Platinum.Products.</param>
        /// <param name="sortedOn"> One of three options to sort Adr list: last updated date, number, title.</param>
        /// <param name="sortAsc"> Denotes whether to sort ascending or descending.</param>
        /// <returns>The collection of grantable activities for the user.</returns>
        /// <response code="200">The grantable activities were returned.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(GitHubAdrSummaryModel), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet("{repoDotName}/details/sorted-on/{sortedOn}/sort-asc/{sortAsc}")]
        public async Task<IActionResult> GitHubGetSpecificAdrSummaries(string repoDotName, string sortedOn, bool sortAsc)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new UXGitHubGetADRsForRepoQuery(
                    tenantId.Value,
                    correlationId.Value,
                    repoDotName,
                    sortedOn,
                    sortAsc);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }
    }
}