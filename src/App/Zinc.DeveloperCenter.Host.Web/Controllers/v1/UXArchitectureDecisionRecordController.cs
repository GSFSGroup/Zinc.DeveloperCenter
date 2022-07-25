using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedLine.Domain;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.UXAdrList.DownloadArchitectureDecisionRecord;
using Zinc.DeveloperCenter.Application.Queries.UXAdrList.GetArchitectureDecisionRecords;
using Zinc.DeveloperCenter.Application.Queries.UXAdrSearch;
using Zinc.DeveloperCenter.Domain.Model.GitHub;

namespace Zinc.DeveloperCenter.Host.Web.Controllers.V1
{
    /// <summary>
    /// An api that provides endpoints for retrieving architecture decision record data.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = ApplicationContext.ApplicationName)]
    [Produces("application/json")]
    [Route("ux/v{version:apiVersion}/{tenantId}/architecture-decision-records")]
    public class UXArchitectureDecisionRecordController : Controller
    {
        private readonly IMediator mediator;
        private readonly ICorrelationId correlationId;
        private readonly ITenantId tenantId;
        private readonly ILogger<UXArchitectureDecisionRecordController> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">Query mediator to interpret protocols.</param>
        /// <param name="correlationId">Unique ID for each request.</param>
        /// <param name="tenantId">Identifier for tenant.</param>
        /// <param name="logger">Diagnostic logger.</param>
        public UXArchitectureDecisionRecordController(
            IMediator mediator,
            ICorrelationId correlationId,
            ITenantId tenantId,
            ILogger<UXArchitectureDecisionRecordController> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId;
            this.tenantId = tenantId;
            this.logger = logger;
        }

        /// <summary>
        /// Downloads a specific ADR defined in an application (repository).
        /// </summary>
        /// <param name="applicationName">The full name of application (repository), e.g. Platinum.Products.</param>
        /// <param name="filePath">The ADR file path in the repository, e.g. docs/App/adr-0001-title.md.</param>
        /// <param name="format">The format to return - 'raw' or 'html'. The default is 'raw'.</param>
        /// <returns>The collection of architecture decision records for the application (repository).</returns>
        /// <response code="200">The request was successful.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [HttpGet("{applicationName}/{filePath}/content")]
        public async Task<IActionResult> DownloadArchitectureDecisionRecord(string applicationName, string filePath, [FromQuery]string? format)
        {
            var fileFormat = FileFormat.Raw;

            if ("html".Equals(format, System.StringComparison.OrdinalIgnoreCase))
            {
                fileFormat = FileFormat.Html;
            }

            return await this.Execute(logger, async () =>
            {
                var request = new UXAdrListDownloadArchitectureDecisionRecordQuery(
                    tenantId.Value,
                    correlationId.Value,
                    applicationName,
                    filePath,
                    fileFormat);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return File(response.Content, response.MimeType, response.FileName);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the list of ADRs defined in an application (repository).
        /// </summary>
        /// <param name="applicationName">The full name of application (repository), e.g. Platinum.Products.</param>
        /// <returns>The collection of architecture decision records for the application (repository).</returns>
        /// <response code="200">The request was successful.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(PageableResult<UXAdrListGetArchitectureDecisionRecordsQueryModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet("{applicationName}")]
        public async Task<IActionResult> GetArchitectureDecisionRecords(string applicationName)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new UXAdrListGetArchitectureDecisionRecordsQuery(
                    tenantId.Value,
                    correlationId.Value,
                    applicationName);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the list of ADRs defined in an application (repository).
        /// </summary>
        /// <param name="q">The search query, or search pattern to use.</param>
        /// <returns>The collection of architecture decision records for the application (repository).</returns>
        /// <response code="200">The request was successful.</response>
        /// <response code="400">A parameter was missing or invalid. The response will contain the error message.</response>
        /// <response code="401">The client is not authenticated.</response>
        /// <response code="403">The client is forbidden to perform the operation.</response>
        /// <response code="500">An unhandled error occurred. The response will contain the error message.</response>
        /// <response code="501">An operation was not implemented.</response>
        [ProducesResponseType(typeof(PageableResult<UXSearchArchitectureDecisionRecordsQueryModel>), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 401)]
        [ProducesResponseType(typeof(string), 403)]
        [ProducesResponseType(typeof(string), 500)]
        [ProducesResponseType(typeof(string), 501)]
        [HttpGet("search")]
        public async Task<IActionResult> SearchArchitectureDecisionRecords([FromQuery]string q)
        {
            return await this.Execute(logger, async () =>
            {
                var request = new UXSearchArchitectureDecisionRecordsQuery(
                    tenantId.Value,
                    correlationId.Value,
                    q);

                var response = await mediator.Send(request).ConfigureAwait(false);

                return Ok(response);
            }).ConfigureAwait(false);
        }
    }
}
