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
    internal class GitHubGetADRsForTemplateRepoQueryHandler : IRequestHandler<GitHubGetADRsForTemplateRepoQuery, PageableResult<GitHubAdrSummaryModel>>
    {
        private readonly IGitHubService gitHubService;
        private readonly ILogger<GitHubGetADRsForTemplateRepoQueryHandler> logger;

        public GitHubGetADRsForTemplateRepoQueryHandler(IGitHubService gitHubService, ILogger<GitHubGetADRsForTemplateRepoQueryHandler> logger)
        {
            this.gitHubService = gitHubService;
            this.logger = logger;
        }

        public async Task<PageableResult<GitHubAdrSummaryModel>> Handle(GitHubGetADRsForTemplateRepoQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Invoke api Proxy to get GitHub adrs");

            var record = await gitHubService
                .GetGitHubAdrData(true)
                .ConfigureAwait(false);

            var adrList = new List<GitHubAdrSummaryModel>();

            foreach (var adrRecord in record.Select(x => x.Name))
            {
                int indexSecondDash = adrRecord.IndexOf('-', adrRecord.IndexOf('-') + 1);
                var nameParts = adrRecord.Split('-');

                var adr = new GitHubAdrSummaryModel
                {
                    NeatTitle = adrRecord.Substring(indexSecondDash, adrRecord.IndexOf('.') - indexSecondDash).Replace('-', ' '),
                    AdrTitle = adrRecord,
                    LastUpdatedDate = "x",
                    Number = Convert.ToInt16(nameParts[1]),
                    NumberString = string.Concat(nameParts[0], '-', nameParts[1]),
                };

                adrList.Add(adr);
            }

            List<GitHubAdrSummaryModel> sortedAdrList = adrList.OrderBy(o => o.Number).ToList();

            return await Task.FromResult(new PageableResult<GitHubAdrSummaryModel>(sortedAdrList)).ConfigureAwait(false);
        }
    }
}