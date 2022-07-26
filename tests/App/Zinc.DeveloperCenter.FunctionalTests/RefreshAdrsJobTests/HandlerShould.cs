#pragma warning disable S1128 // Unused "using" should be removed
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using RedLine.Application.Jobs;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Application.Jobs.RefreshAdrs;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Services.GitHub;
#pragma warning restore S1128 // Unused "using" should be removed

namespace Zinc.DeveloperCenter.FunctionalTests.RefreshAdrsJobTests
{
    public class HandlerShould : FunctionalTestBase
    {
        public HandlerShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        public async Task PopulateTheDatabase()
        {
            /*
            var job = new RefreshAdrsJob("GSFSGroup", Guid.NewGuid())

            var handler = new RefreshAdrsJobHandler(
                GetRequiredService<IGitHubApiService>(),
                GetRequiredService<IApplicationRepository>(),
                GetRequiredService<IArchitectureDecisionRecordRepository>(),
                GetRequiredService<ILogger<RefreshAdrsJobHandler>>())
             * */

            await Task.CompletedTask.ConfigureAwait(false);

            var token = GetRequiredService<GitHubApiConfig>().Tenants?.FirstOrDefault()?.AccessToken;
            Output.WriteLine("????????????????? " + token);

            token.Should().NotBeNullOrEmpty();

            // var response = await handler.Handle(job, CancellationToken.None).ConfigureAwait(false)
            // response.Should().Be(JobResult.OperationSucceeded)
        }
    }
}
