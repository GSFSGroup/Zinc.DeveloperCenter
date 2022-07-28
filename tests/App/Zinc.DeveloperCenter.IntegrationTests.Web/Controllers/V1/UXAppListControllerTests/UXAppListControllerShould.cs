using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using RedLine.Domain.Model;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.UXAppList.GetApplications;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.UXAppListControllerTests
{
    public class UXAppListControllerShould : WebTestBase
    {
        private readonly string endpoint = $"/ux/v1/{TenantId}/applications";

        public UXAppListControllerShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ReturnAllApplications()
        {
            // Arrange
            await InsertData().ConfigureAwait(false);

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

        private async Task InsertData()
        {
            await Mothers.TestData.InsertData(
                TenantId,
                GetRequiredService<IApplicationRepository>(),
                GetRequiredService<IArchitectureDecisionRecordRepository>()).ConfigureAwait(false);
        }
    }
}
