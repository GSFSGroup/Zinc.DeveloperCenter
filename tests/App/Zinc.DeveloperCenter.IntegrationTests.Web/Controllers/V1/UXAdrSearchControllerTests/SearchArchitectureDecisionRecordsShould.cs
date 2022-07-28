using System.Linq;
using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using RedLine.Domain.Model;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.UXAdrSearch;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.UXAdrSearchControllerTests
{
    public class SearchArchitectureDecisionRecordsShould : WebTestBase
    {
        private readonly string endpoint = $"/ux/v1/{TenantId}/architecture-decision-records/search";

        public SearchArchitectureDecisionRecordsShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task PerformFullTextSearch()
        {
            // Arrange
            await InsertData().ConfigureAwait(false);
            var query1 = "technological & political & social";
            var query2 = "event & sourcing";

            // Act
            var response1 = await AuthorizedScenario(_ =>
            {
                _.Get.Url(endpoint).QueryString("q", query1);
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            var response2 = await AuthorizedScenario(_ =>
            {
                _.Get.Url(endpoint).QueryString("q", query2);
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            // Assert
            var result1 = response1.ReadAsJson<PageableResult<UXSearchArchitectureDecisionRecordsQueryModel>>();
            var result2 = response2.ReadAsJson<PageableResult<UXSearchArchitectureDecisionRecordsQueryModel>>();

            result1.Should().NotBeNull();
            result1.Items.Should().HaveCount(1);
            result1.Items.First().Title.Should().Be("record-architecture-decisions");

            result2.Should().NotBeNull();
            result2.Items.Should().HaveCount(1);
            result2.Items.First().Title.Should().Be("event-sourcing");
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
