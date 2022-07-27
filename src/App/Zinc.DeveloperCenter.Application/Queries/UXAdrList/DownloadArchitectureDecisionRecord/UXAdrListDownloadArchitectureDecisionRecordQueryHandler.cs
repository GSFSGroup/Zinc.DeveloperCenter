using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Zinc.DeveloperCenter.Domain.Model;
using Zinc.DeveloperCenter.Domain.Services.GitHub;
using Zinc.DeveloperCenter.Domain.Services.MostViewed;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrList.DownloadArchitectureDecisionRecord
{
    internal class UXAdrListDownloadArchitectureDecisionRecordQueryHandler : IRequestHandler<UXAdrListDownloadArchitectureDecisionRecordQuery, UXAdrListDownloadArchitectureDecisionRecordQueryModel>
    {
        private readonly IGitHubApiService gitHubApi;
        private readonly IMostViewedService mostViewedService;
        private readonly ILogger<UXAdrListDownloadArchitectureDecisionRecordQueryHandler> logger;

        public UXAdrListDownloadArchitectureDecisionRecordQueryHandler(
            IGitHubApiService gitHubApi,
            IMostViewedService mostViewedService,
            ILogger<UXAdrListDownloadArchitectureDecisionRecordQueryHandler> logger)
        {
            this.gitHubApi = gitHubApi;
            this.mostViewedService = mostViewedService;
            this.logger = logger;
        }

        public async Task<UXAdrListDownloadArchitectureDecisionRecordQueryModel> Handle(UXAdrListDownloadArchitectureDecisionRecordQuery request, CancellationToken cancellationToken)
        {
            var contentModel = await gitHubApi.DownloadArchitectureDecisionRecord(
                request.TenantId,
                request.ApplicationName,
                request.FilePath,
                request.FileFormat).ConfigureAwait(false);

            if (string.IsNullOrEmpty(contentModel.Content))
            {
                throw new RedLine.Data.Exceptions.ResourceNotFoundException(nameof(ArchitectureDecisionRecord), string.Join('/', request.TenantId, request.ApplicationName, request.FilePath));
            }

            var fileName = System.IO.Path.GetFileName(request.FilePath);
            var fileMimeType = request.FileFormat == FileFormat.Raw
                ? "text/markdown"
                : "text/html";

            try
            {
                await mostViewedService.UpdateViewCount(request.TenantId, request.ApplicationName, request.FilePath)
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
                contentModel.Content,
                contentModel.ContentUrl!);
        }
    }
}
