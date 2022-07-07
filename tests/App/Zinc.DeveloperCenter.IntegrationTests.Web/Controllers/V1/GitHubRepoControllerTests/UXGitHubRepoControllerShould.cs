using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using RedLine.Domain.Model;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.GitHubRepo.Models;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.UXGitHubRepoControllerTests
{
    public class UXGitHubRepoControllerShould : WebTestBase
    {
        private readonly string endpoint = $"/ux/v1/{TenantId}/repos";

        public UXGitHubRepoControllerShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
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
            var result = response.ReadAsJson<PageableResult<GitHubRepoModel>>();
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
        }
    }
}