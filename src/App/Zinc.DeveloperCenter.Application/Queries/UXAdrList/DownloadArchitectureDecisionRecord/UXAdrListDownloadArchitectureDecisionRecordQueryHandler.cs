using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Zinc.DeveloperCenter.Domain.Model;
using Zinc.DeveloperCenter.Domain.Services.GitHub;
using Zinc.DeveloperCenter.Domain.Services.ViewCounter;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrList.DownloadArchitectureDecisionRecord
{
    internal class UXAdrListDownloadArchitectureDecisionRecordQueryHandler : IRequestHandler<UXAdrListDownloadArchitectureDecisionRecordQuery, UXAdrListDownloadArchitectureDecisionRecordQueryModel>
    {
        private readonly IGitHubApiService gitHubApi;
        private readonly IViewCounterService viewCounterService;
        private readonly ILogger<UXAdrListDownloadArchitectureDecisionRecordQueryHandler> logger;

        public UXAdrListDownloadArchitectureDecisionRecordQueryHandler(
            IGitHubApiService gitHubApi,
            IViewCounterService viewCounterService,
            ILogger<UXAdrListDownloadArchitectureDecisionRecordQueryHandler> logger)
        {
            this.gitHubApi = gitHubApi;
            this.viewCounterService = viewCounterService;
            this.logger = logger;
        }

        public async Task<UXAdrListDownloadArchitectureDecisionRecordQueryModel> Handle(UXAdrListDownloadArchitectureDecisionRecordQuery request, CancellationToken cancellationToken)
        {
            var content = await gitHubApi.DownloadArchitectureDecisionRecord(
                request.TenantId,
                request.ApplicationName,
                request.FilePath,
                request.FileFormat).ConfigureAwait(false);

            if (content?.Length == 0)
            {
                throw new RedLine.Data.Exceptions.ResourceNotFoundException(nameof(ArchitectureDecisionRecord), string.Join('/', request.TenantId, request.ApplicationName, request.FilePath));
            }

            // Force all file names to end with .md vs .markdown (technical UI reasons)
            var fileName = $"{System.IO.Path.GetFileNameWithoutExtension(request.FilePath)}.md";

            var fileMimeType = request.FileFormat == FileFormat.Raw
                ? "text/markdown"
                : "text/html";

            var fileContent = System.Text.Encoding.UTF8.GetBytes(content!);

            try
            {
                await viewCounterService.UpdateViewCount(request.TenantId, request.ApplicationName, request.FilePath)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // No need to stop the download because of this error so just log it
                logger.LogError(
                    e,
                    "[APPL]==> {Error} updating the view count for ADR '{ADR}'.\n[APPL]<== ERROR 500: {Message}",
                    e.GetType().Name,
                    string.Join('/', request.ApplicationName, request.FilePath),
                    e.Message);
            }

            return new UXAdrListDownloadArchitectureDecisionRecordQueryModel(
                fileName,
                fileMimeType,
                fileContent);
        }
    }
}
