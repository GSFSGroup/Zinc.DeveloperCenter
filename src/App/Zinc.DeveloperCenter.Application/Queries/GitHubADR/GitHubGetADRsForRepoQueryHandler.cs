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
    internal class GitHubGetADRsForRepoQueryHandler : IRequestHandler<GitHubGetADRsForRepoQuery, PageableResult<GitHubAdrSummaryModel>>
    {
        private readonly IGitHubService gitHubService;
        private readonly ILogger<GitHubGetADRsForRepoQueryHandler> logger;

        public GitHubGetADRsForRepoQueryHandler(IGitHubService gitHubService, ILogger<GitHubGetADRsForRepoQueryHandler> logger)
        {
            this.gitHubService = gitHubService;
            this.logger = logger;
        }

        public async Task<PageableResult<GitHubAdrSummaryModel>> Handle(GitHubGetADRsForRepoQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Invoke api Proxy to get GitHub adrs");

            int page = 1;

            var record = await gitHubService
                .GetGitHubAdrData(false)
                .ConfigureAwait(false);

            // pagination solution.
            // GitHub can query 100 repos at a time.
            // This repeatedly grabs 100 repos until reaching the last page.
            while (true)
            {
                page++;

                var adrToAppend = await gitHubService
                    .GetGitHubAdrData(false)
                    .ConfigureAwait(false);

                record.AddRange(adrToAppend);

                if (adrToAppend.Count < 100)
                {
                    break;
                }
            }

            var adrList = new List<GitHubAdrSummaryModel>();

            foreach (var adrRecord in record.Select(x => x.Name))
            {
                var nameParts = adrRecord.Split('-', 2);

                var adr = new GitHubAdrSummaryModel
                {
                    NeatTitle = adrRecord.Replace('-', ' '),
                    AdrTitle = adrRecord,
                    LastUpdatedDate = "x",
                    Number = Convert.ToInt16(nameParts[0].Split('-')[1]),
                    NumberString = nameParts[0],
                };

                adrList.Add(adr);
            }

            List<GitHubAdrSummaryModel> sortedAdrList = adrList.OrderBy(o => o.Number).ToList();

            return await Task.FromResult(new PageableResult<GitHubAdrSummaryModel>(sortedAdrList)).ConfigureAwait(false);
        }
    }
}