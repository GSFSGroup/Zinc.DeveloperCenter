using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using RedLine.Domain.Model;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.UXAppList.GetApplications;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.UXAppListControllerTests
{
    public class UXAppListControllerShould : WebTestBase
    {
        private readonly string endpoint = $"/ux/v1/GSFSGroup/applications";

        public UXAppListControllerShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ReturnAllApplications()
        {
            // Act
            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{endpoint}");
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            // Assert
            var result = response.ReadAsJson<PageableResult<UXAppListGetApplicationsQueryModel>>();
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
        }
    }
}
