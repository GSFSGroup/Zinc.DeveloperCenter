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

            var repoList = await gitHubService
                .GetGitHubRepoData()
                .ConfigureAwait(false);

            List<GitHubRepoModel> sortedRepoList = repoList.OrderBy(o => o.ApplicationDisplayName).ToList();

            return new PageableResult<GitHubRepoModel>(sortedRepoList);
        }
    }
}