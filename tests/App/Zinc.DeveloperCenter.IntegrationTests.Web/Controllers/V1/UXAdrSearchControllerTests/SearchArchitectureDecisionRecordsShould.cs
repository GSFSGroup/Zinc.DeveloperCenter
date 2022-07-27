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
            var query1 = System.Web.HttpUtility.UrlEncode("technological & political & social");
            var query2 = System.Web.HttpUtility.UrlEncode("event & sourcing");

            // Act
            var response1 = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{endpoint}?q={query1}");
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            var response2 = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{endpoint}?q={query2}");
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
        }
    }
}
