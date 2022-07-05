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

            var record = await gitHubService
                .GetGitHubAdrData(request.RepoDotName)
                .ConfigureAwait(false);

            var adrList = new List<GitHubAdrSummaryModel>();

            foreach (var adrRecord in record.Select(x => x.Name))
            {
                // add a file to the list if it is an adr.
                // an adr will begin with "adr" and end with ".md" or ".markdown".
                if ((adrRecord.Length > 4 && adrRecord.Substring(0, 3) == "adr") && (adrRecord.Substring(adrRecord.Length - 3) == ".md" || (adrRecord.Length > 10 && adrRecord.Substring(adrRecord.Length - 9) == ".markdown")))
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
            }

            List<GitHubAdrSummaryModel> sortedAdrList = adrList.OrderBy(o => o.Number).ToList();

            return new PageableResult<GitHubAdrSummaryModel>(sortedAdrList);
        }
    }
}
