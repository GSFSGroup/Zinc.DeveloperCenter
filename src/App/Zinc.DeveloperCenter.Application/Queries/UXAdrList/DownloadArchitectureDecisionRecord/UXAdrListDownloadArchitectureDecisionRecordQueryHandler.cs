using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MediatR;
using Zinc.DeveloperCenter.Domain.Model;
using Zinc.DeveloperCenter.Domain.Services.GitHub;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrList.DownloadArchitectureDecisionRecord
{
    internal class UXAdrListDownloadArchitectureDecisionRecordQueryHandler : IRequestHandler<UXAdrListDownloadArchitectureDecisionRecordQuery, UXAdrListDownloadArchitectureDecisionRecordQueryModel>
    {
        private readonly IGitHubApiService gitHubApi;

        public UXAdrListDownloadArchitectureDecisionRecordQueryHandler(IGitHubApiService gitHubApi)
        {
            this.gitHubApi = gitHubApi;
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

            /* TODO
            await connection.ExecuteAsync(
                @"UPDATE developercenter.architecture_decision_record_viewcount
                  SET view_count = view_count + 1
                  WHERE id = (SELECT id FROM developercenter.architecture_decision_record
                              WHERE tenant_id = @TenantId AND application_name = @ApplicationName AND file_path = @FilePath)",
                new { request.TenantId, request.ApplicationName, request.FilePath }).ConfigureAwait(false)
             * */

            return new UXAdrListDownloadArchitectureDecisionRecordQueryModel(
                fileName,
                fileMimeType,
                fileContent);
        }
    }
}
