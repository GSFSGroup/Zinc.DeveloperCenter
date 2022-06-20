using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Application.Queries.GitHubRepo.Models;

namespace Zinc.DeveloperCenter.Application.Queries.GitHubRepo
{
    internal class GitHubGetReposQueryHandler : IRequestHandler<GitHubGetReposQuery, PageableResult<GitHubRepoModel>>
    {
        public async Task<PageableResult<GitHubRepoModel>> Handle(GitHubGetReposQuery request, CancellationToken cancellationToken)
        {
            var repoList = Enumerable.Repeat(0, 20).Select(h => new GitHubRepoModel
            {
                DotName = "Test.TestRepo",
                NeatName = "TestRepo",
                Element = "Test",
                ContentURL = "example.com",
            }).ToArray();
            return await Task.Run(() => new PageableResult<GitHubRepoModel>(repoList)).ConfigureAwait(false);
        }
    }
}