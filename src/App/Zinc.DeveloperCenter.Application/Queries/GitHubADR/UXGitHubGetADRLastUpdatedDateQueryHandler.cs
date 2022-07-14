using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Zinc.DeveloperCenter.Application.Services;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR
{
    internal class UXGitHubGetAdrLastUpdatedDateQueryHandler : IRequestHandler<UXGitHubGetAdrLastUpdatedDateQuery, DateTime>
    {
        private readonly IGitHubService gitHubService;
        private readonly ILogger<UXGitHubGetAdrLastUpdatedDateQueryHandler> logger;

        public UXGitHubGetAdrLastUpdatedDateQueryHandler(IGitHubService gitHubService, ILogger<UXGitHubGetAdrLastUpdatedDateQueryHandler> logger)
        {
            this.gitHubService = gitHubService;
            this.logger = logger;
        }

        public async Task<DateTime> Handle(UXGitHubGetAdrLastUpdatedDateQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Invoke api Proxy to get Adr update times from GitHub.");

            var record = await gitHubService
                .GetAdrLastUpdatedData(request.RepoDotName, request.AdrTitle)
                .ConfigureAwait(false);

            return record;
        }
    }
}
