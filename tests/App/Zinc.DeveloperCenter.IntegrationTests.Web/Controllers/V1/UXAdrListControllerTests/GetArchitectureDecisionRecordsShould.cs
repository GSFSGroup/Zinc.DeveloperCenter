using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using RedLine.Domain.Model;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.UXAdrList.GetArchitectureDecisionRecords;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.UXAdrListControllerTests
{
    public class GetArchitectureDecisionRecordsShould : WebTestBase
    {
        private readonly string endpoint = $"/ux/v1/{TenantId}/architecture-decision-records";

        public GetArchitectureDecisionRecordsShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task ReturnTheADRs()
        {
            // Arrange
            await InsertData().ConfigureAwait(false);

            // Act
            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{endpoint}/Zinc.Templates");
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            // Assert
            var result = response.ReadAsJson<PageableResult<UXAdrListGetArchitectureDecisionRecordsQueryModel>>();
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(3);
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
