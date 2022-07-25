using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using RedLine.Domain.Model;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.UXAdrList.GetArchitectureDecisionRecords;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.UXAdrListArchitectureDecisionRecordControllerTests
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
            result.Items.Should().HaveCount(1);
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