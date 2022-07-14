using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;
using Zinc.DeveloperCenter.Application.Services;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR
{
    internal class UXGitHubGetAdrLastUpdatedDateQueryHandler : IRequestHandler<UXGitHubGetAdrLastUpdatedDateQuery, GitHubAdrLastUpdatedDateModel>
    {
        private readonly IGitHubService gitHubService;
        private readonly ILogger<UXGitHubGetAdrLastUpdatedDateQueryHandler> logger;

        public UXGitHubGetAdrLastUpdatedDateQueryHandler(IGitHubService gitHubService, ILogger<UXGitHubGetAdrLastUpdatedDateQueryHandler> logger)
        {
            this.gitHubService = gitHubService;
            this.logger = logger;
        }

        public async Task<GitHubAdrLastUpdatedDateModel> Handle(UXGitHubGetAdrLastUpdatedDateQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Invoke api Proxy to get Adr update times from GitHub.");

            var record = await gitHubService
                .GetAdrLastUpdatedData(request.RepoDotName, request.AdrTitle)
                .ConfigureAwait(false);

            var dateToAdd = new GitHubAdrLastUpdatedDateModel
            {
                LastUpdatedDate = record.Date,
            };

            return dateToAdd;
        }
    }
}
