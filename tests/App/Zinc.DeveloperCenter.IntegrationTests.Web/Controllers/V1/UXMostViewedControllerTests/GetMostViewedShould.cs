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
            var applicationRepository = GetRequiredService<IApplicationRepository>();
            var adrRepository = GetRequiredService<IArchitectureDecisionRecordRepository>();

            await applicationRepository.Save(new Domain.Model.Application(
                TenantId,
                "Zinc.Templates",
                "https://github.com/GSFSGroup/Zinc.Templates",
                "A template for new projects... .NET projects to be specific")).ConfigureAwait(false);

            await applicationRepository.Save(new Domain.Model.Application(
                TenantId,
                "Molybdenum.Earnings",
                "https://github.com/GSFSGroup/Molybdenum.Earnings",
                "A service to calculate earnings based on curves for insurance and insurance-like financial products.")).ConfigureAwait(false);

            await adrRepository.Save(new Domain.Model.ArchitectureDecisionRecord(
                TenantId,
                "Zinc.Templates",
                "dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md",
                "Homer Simpson",
                System.DateTimeOffset.UtcNow.AddDays(-30),
                Data.Migrations.EmbeddedResources.EmbeddedResource.Read("Migration_2022072101_TestData_adr_01.md"))).ConfigureAwait(false);

            await adrRepository.Save(new Domain.Model.ArchitectureDecisionRecord(
                TenantId,
                "Molybdenum.Earnings",
                "docs/App/adr-0001-event-sourcing.md",
                "Marge Simpson",
                System.DateTimeOffset.UtcNow.AddDays(-10),
                Data.Migrations.EmbeddedResources.EmbeddedResource.Read("Migration_2022072101_TestData_adr_04.md"))).ConfigureAwait(false);

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
