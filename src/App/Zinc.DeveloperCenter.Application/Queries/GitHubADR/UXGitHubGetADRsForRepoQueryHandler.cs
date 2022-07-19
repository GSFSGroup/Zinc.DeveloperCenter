using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.GitHubADR.Models;
using Zinc.DeveloperCenter.Application.Services;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubADR
{
    internal class UXGitHubGetADRsForRepoQueryHandler : IRequestHandler<UXGitHubGetADRsForRepoQuery, PageableResult<GitHubAdrSummaryModel>>
    {
        private readonly IGitHubService gitHubService;
        private readonly ILogger<UXGitHubGetADRsForRepoQueryHandler> logger;

        public UXGitHubGetADRsForRepoQueryHandler(IGitHubService gitHubService, ILogger<UXGitHubGetADRsForRepoQueryHandler> logger)
        {
            this.gitHubService = gitHubService;
            this.logger = logger;
        }

        public async Task<PageableResult<GitHubAdrSummaryModel>> Handle(UXGitHubGetADRsForRepoQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Invoke api Proxy to get GitHub adrs");

            var adrList = await gitHubService
                .GetGitHubAdrData(request.RepoDotName)
                .ConfigureAwait(false);

            List<GitHubAdrSummaryModel> sortedAdrList;

            if (request.SortedOn.Equals("lud"))
            {
                if (request.SortAsc)
                {
                    sortedAdrList = adrList.OrderBy(o => o.LastUpdatedDate).ToList();
                }
                else
                {
                    sortedAdrList = adrList.OrderByDescending(o => o.LastUpdatedDate).ToList();
                }
            }
            else if (request.SortedOn.Equals("number"))
            {
                if (request.SortAsc)
                {
                    sortedAdrList = adrList.OrderBy(o => o.Number).ToList();
                }
                else
                {
                    sortedAdrList = adrList.OrderByDescending(o => o.Number).ToList();
                }
            }
            else
            {
                if (request.SortAsc)
                {
                    sortedAdrList = adrList.OrderBy(o => o.NeatTitle).ToList();
                }
                else
                {
                    sortedAdrList = adrList.OrderByDescending(o => o.NeatTitle).ToList();
                }
            }

            return new PageableResult<GitHubAdrSummaryModel>(sortedAdrList);
        }
    }
}
