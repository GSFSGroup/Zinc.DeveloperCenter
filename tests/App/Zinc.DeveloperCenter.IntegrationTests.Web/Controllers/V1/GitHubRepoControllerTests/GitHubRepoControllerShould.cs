using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.GitHubRepo.Models;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.GithubRepoControllerTests
{
    public class GitHubRepoControllerShould : WebTestBase
    {
        private readonly string endpoint = $"/ux/v1/{TenantId}/repos";
        private readonly List<GitHubRepoModel> repos;

        public GitHubRepoControllerShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
            repos = GetRequiredService<IEnumerable<GitHubRepoModel>>().ToList();
        }

        [Fact]
        public async Task ReturnAllRepos()
        {
            // Act
            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{endpoint}");
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            // Assert
            var result = response.ReadAsJson<IEnumerable<GitHubRepoModel>>();
            result.Should().NotBeNull();
            result.Should().HaveCount(repos.Count);
            repos.ForEach(a =>
                result.Should().Contain(r =>
                    r.DotName == a.DotName &&
                    r.NeatName == a.NeatName &&
                    r.Element == a.Element &&
                    r.ContentURL == a.ContentURL));
        }
    }
}