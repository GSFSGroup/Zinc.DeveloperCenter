using System.Threading.Tasks;
using Alba;
using FluentAssertions;
using RedLine.Domain.Model;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Queries.UXFavorites;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Host.Web.Models;

namespace Zinc.DeveloperCenter.IntegrationTests.Web.Controllers.V1.FavoritesControllerTests
{
    public class FavoritesControllerShould : WebTestBase
    {
        private readonly string apiEndpoint = $"/api/v1/{TenantId}/architecture-decision-records/favorites";
        private readonly string uxEndpoint = $"/ux/v1/{TenantId}/architecture-decision-records/favorites";

        public FavoritesControllerShould(WebTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task AddTheFavorite()
        {
            // Arrange
            await InsertData().ConfigureAwait(false);
            var model = new AddFavoriteModel { ApplicationName = "Zinc.Templates", FilePath = "dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md" };

            // Act
            await AuthorizedScenario(_ =>
            {
                _.Post.Json(model).ToUrl(apiEndpoint);
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);

            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url(uxEndpoint);
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            // Assert
            var responseModel = response.ReadAsJson<PageableResult<UXGetFavoritesQueryModel>>();
            responseModel.Should().NotBeNull();
            responseModel.Items.Count.Should().Be(1);
        }

        [Fact]
        public async Task RemoveTheFavorite()
        {
            // Arrange
            await InsertData().ConfigureAwait(false);
            var model = new AddFavoriteModel { ApplicationName = "Zinc.Templates", FilePath = "dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md" };

            // Act
            await AuthorizedScenario(_ =>
            {
                _.Post.Json(model).ToUrl(apiEndpoint);
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);

            var response = await AuthorizedScenario(_ =>
            {
                _.Get.Url(uxEndpoint);
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            // Assert
            var responseModel = response.ReadAsJson<PageableResult<UXGetFavoritesQueryModel>>();
            responseModel.Should().NotBeNull();
            responseModel.Items.Count.Should().Be(1);

            // Act
            await AuthorizedScenario(_ =>
            {
                _.Delete.Url($"{apiEndpoint}/Zinc.Templates").QueryString("path", "dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md");
                _.StatusCodeShouldBe(204);
            }).ConfigureAwait(false);

            response = await AuthorizedScenario(_ =>
            {
                _.Get.Url(uxEndpoint);
                _.StatusCodeShouldBeOk();
            }).ConfigureAwait(false);

            // Assert
            responseModel = response.ReadAsJson<PageableResult<UXGetFavoritesQueryModel>>();
            responseModel.Should().NotBeNull();
            responseModel.Items.Count.Should().Be(0);
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
