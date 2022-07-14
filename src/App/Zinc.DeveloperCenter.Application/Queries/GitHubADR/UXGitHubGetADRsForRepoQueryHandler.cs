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

            var record = await gitHubService
                .GetGitHubAdrData(request.RepoDotName)
                .ConfigureAwait(false);

            var adrList = new List<GitHubAdrSummaryModel>();

            foreach (var adrRecord in record)
            {
                // add a file to the list if it is an adr.
                // an adr will begin with "adr" and end with ".md" or ".markdown".
                if ((adrRecord.Name.Length > 4 && adrRecord.Name.Substring(0, 3) == "adr") && (adrRecord.Name.Substring(adrRecord.Name.Length - 3) == ".md" || (adrRecord.Name.Length > 10 && adrRecord.Name.Substring(adrRecord.Name.Length - 9) == ".markdown")))
                {
                    int indexSecondDash = adrRecord.Name.IndexOf('-', adrRecord.Name.IndexOf('-') + 1);
                    var nameParts = adrRecord.Name.Split('-');

                    var adr = new GitHubAdrSummaryModel
                    {
                        NeatTitle = adrRecord.Name.Substring(indexSecondDash, adrRecord.Name.IndexOf('.') - indexSecondDash).Replace('-', ' '),
                        AdrTitle = adrRecord.Name,
                        LastUpdatedDate = string.Empty,
                        Number = Convert.ToInt16(nameParts[1]),
                        NumberString = string.Concat(nameParts[0], '-', nameParts[1]),
                        DownloadUrl = adrRecord.DownloadUrl,
                        HtmlUrl = adrRecord.HtmlUrl,
                        FilePath = adrRecord.FilePath,
                    };

                    adrList.Add(adr);
                }
            }

            List<GitHubAdrSummaryModel> sortedAdrList = adrList.OrderBy(o => o.Number).ToList();

            return new PageableResult<GitHubAdrSummaryModel>(sortedAdrList);
        }
    }
}
