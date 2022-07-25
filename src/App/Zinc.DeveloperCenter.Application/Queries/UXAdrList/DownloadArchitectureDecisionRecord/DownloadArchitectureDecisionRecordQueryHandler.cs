using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Zinc.DeveloperCenter.Domain.Model;
using Zinc.DeveloperCenter.Domain.Model.GitHub;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrList.DownloadArchitectureDecisionRecord
{
    internal class DownloadArchitectureDecisionRecordQueryHandler : IRequestHandler<DownloadArchitectureDecisionRecordQuery, DownloadArchitectureDecisionRecordQueryModel>
    {
        private readonly IGitHubApiService gitHubApi;

        public DownloadArchitectureDecisionRecordQueryHandler(IGitHubApiService gitHubApi)
        {
            this.gitHubApi = gitHubApi;
        }

        public async Task<DownloadArchitectureDecisionRecordQueryModel> Handle(DownloadArchitectureDecisionRecordQuery request, CancellationToken cancellationToken)
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
            var fileContent = System.Text.Encoding.UTF8.GetBytes(content);

            return new DownloadArchitectureDecisionRecordQueryModel(fileName, fileContent);
        }
    }
}
