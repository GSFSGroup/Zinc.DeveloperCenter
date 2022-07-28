using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.UXAdrList.DownloadArchitectureDecisionRecord;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Services.MostViewed;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.UXAdrListControllerTests
{
    public class DownloadArchitectureDecisionRecordShould : WebTestBase
    {
        private readonly string endpoint = $"/ux/v1/{TenantId}/architecture-decision-records";

        public DownloadArchitectureDecisionRecordShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task DownloadAsMarkdownAndUpdateViewCount()
        {
            // Arrange
            await InsertData().ConfigureAwait(false);
            var applicationName = "Zinc.Templates";
            var filePath = "dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md";

            // Act
            var response1 = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{endpoint}/download/{applicationName}").QueryString("path", filePath);
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            var response2 = await AuthorizedScenario(_ =>
            {
                _.Get.Url($"{endpoint}/download/{applicationName}").QueryString("path", filePath);
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            // Assert
            var result1 = response1.ReadAsJson<UXAdrListDownloadArchitectureDecisionRecordQueryModel>();
            result1.Content.Should().NotBeEmpty();
            result1.ContentUrl.Should().NotBeEmpty();

            var result2 = response2.ReadAsJson<UXAdrListDownloadArchitectureDecisionRecordQueryModel>();
            result2.Content.Should().NotBeEmpty();
            result2.ContentUrl.Should().NotBeEmpty();

            var viewCount = await GetRequiredService<IMostViewedService>().GetViewCount(TenantId, applicationName, filePath).ConfigureAwait(false);
            viewCount.Should().Be(2);

            (await GetRequiredService<IArchitectureDecisionRecordRepository>()
                .Read(string.Join('/', TenantId, applicationName, filePath))
                .ConfigureAwait(false))
                .TotalViews.Should().Be(2);
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
