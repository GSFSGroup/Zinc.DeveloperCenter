using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Domain.Services.GitHub;

namespace Zinc.DeveloperCenter.FunctionalTests.Application.GitHubApiServiceTests
{
    public class GetLastUpdatedDetailsShould : FunctionalTestBase
    {
        public GetLastUpdatedDetailsShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        { }

        [Fact]
        public async Task ReturnLastUpdatedDetails()
        {
            var api = GetRequiredService<IGitHubApiService>();

            var lastUpdatedDetails = await api.GetLastUpdatedDetails(
                "GSFSGroup",
                "Zinc.Templates",
                "dotnet-5.0/docs/RedLine/adr-0001-record-architecture-decisions.md").ConfigureAwait(false);

            lastUpdatedDetails.LastUpdatedBy.Should().NotBeNull();
            lastUpdatedDetails.LastUpdatedOn.Should().NotBeNull();
        }
    }
}
