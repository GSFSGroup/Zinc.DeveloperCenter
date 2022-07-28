using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Alba;
using Dapper;
using FluentAssertions;
using RedLine.Domain.Model;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.UXMostUsed.GetMostViewed;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.UXMostViewedControllerTests
{
    public class GetMostViewedShould : WebTestBase
    {
        private readonly string endpoint = $"/ux/v1/{TenantId}/architecture-decision-records/most-viewed";

        public GetMostViewedShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        { }

        [Fact]
        public async Task ReturnTheMostViewedADRs()
        {
            // Arrange
            await InsertData().ConfigureAwait(false);

            // Act
            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url(endpoint);
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            // Assert
            var result = response.ReadAsJson<PageableResult<UXGetMostViewedQueryModel>>();
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(1);
            result.Items.First().FilePath.Should().Be("docs/App/adr-0001-event-sourcing.md");
            result.Items.First().TotalViews.Should().Be(2);
        }

        private async Task InsertData()
        {
            await Mothers.TestData.InsertData(
                TenantId,
                GetRequiredService<IApplicationRepository>(),
                GetRequiredService<IArchitectureDecisionRecordRepository>()).ConfigureAwait(false);

            await GetRequiredService<IDbConnection>().ExecuteAsync(
                @$"
INSERT INTO developercenter.architecture_decision_record_viewcount (
    id,
    view_count
) VALUES (@
    (
     SELECT id FROM developercenter.architecture_decision_record
     WHERE tenant_id = '{TenantId}'
     AND application_name = 'Molybdenum.Earnings'
     AND file_path = 'docs/App/adr-0001-event-sourcing.md'
    ),
    @viewCount
);",
                new { viewCount = 2 }).ConfigureAwait(false);
        }
    }
}
