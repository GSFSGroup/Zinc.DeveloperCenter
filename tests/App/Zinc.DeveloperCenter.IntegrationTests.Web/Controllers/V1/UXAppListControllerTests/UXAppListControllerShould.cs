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
            var repository = GetRequiredService<IApplicationRepository>();

            await repository.Save(new Domain.Model.Application(
                TenantId,
                "Zinc.Templates",
                "https://github.com/GSFSGroup/Zinc.Templates",
                "A template for new projects... .NET projects to be specific")).ConfigureAwait(false);

            await repository.Save(new Domain.Model.Application(
                TenantId,
                "Molybdenum.Earnings",
                "https://github.com/GSFSGroup/Zinc.Templates",
                "A service to calculate earnings based on curves for insurance and insurance-like financial products.")).ConfigureAwait(false);
        }
    }
}
