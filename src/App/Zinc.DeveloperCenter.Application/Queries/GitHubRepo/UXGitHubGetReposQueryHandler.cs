using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.GitHubRepo.Models;
using Zinc.DeveloperCenter.Application.Services;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubRepo
{
    internal class UXGitHubGetReposQueryHandler : IRequestHandler<UXGitHubGetReposQuery, PageableResult<GitHubRepoModel>>
    {
        private readonly IGitHubService gitHubService;
        private readonly ILogger<UXGitHubGetReposQueryHandler> logger;

        public UXGitHubGetReposQueryHandler(IGitHubService gitHubService, ILogger<UXGitHubGetReposQueryHandler> logger)
        {
            this.gitHubService = gitHubService;
            this.logger = logger;
        }

        public async Task<PageableResult<GitHubRepoModel>> Handle(UXGitHubGetReposQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Invoke api Proxy to get GitHub repos");

            int page = 1;

            var record = await gitHubService
                .GetGitHubRepoData(page)
                .ConfigureAwait(false);

            // pagination solution.
            // GitHub can query 100 repos at a time.
            // This repeatedly grabs 100 repos until reaching the last page.
            while (true)
            {
                page++;

                var recordToAppend = await gitHubService
                    .GetGitHubRepoData(page)
                    .ConfigureAwait(false);

                record.AddRange(recordToAppend);

                if (recordToAppend.Count < 100)
                {
                    break;
                }
            }

            var repoList = new List<GitHubRepoModel>();

            foreach (GitHubRepoRecord repoRecord in record)
            {
                var nameParts = repoRecord.Name.Split('.');
                var element = nameParts[0];
                var neatName = string.Join(".", nameParts.Skip(1));

                // a few repos do not contain periods,
                // and their neatName will be stored as their element.
                // this swaps the two strings for such repos.
                if (string.IsNullOrEmpty(neatName))
                {
                    neatName = repoRecord.Name;
                    element = string.Empty;
                }

                var repo = new GitHubRepoModel
                {
                    DotName = repoRecord.Name,
                    NeatName = neatName,
                    Element = element,
                    ContentURL = repoRecord.ContentsUrl,
                };

                repoList.Add(repo);
            }

            List<GitHubRepoModel> sortedRepoList = repoList.OrderBy(o => o.NeatName).ToList();

            return new PageableResult<GitHubRepoModel>(sortedRepoList);
        }
    }
}
