using System;
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
            var job = new RefreshAdrsJob("GSFSGroup", Guid.NewGuid());

            var handler = new RefreshAdrsJobHandler(
                GetRequiredService<IGitHubApiService>(),
                GetRequiredService<IApplicationRepository>(),
                GetRequiredService<IArchitectureDecisionRecordRepository>(),
                GetRequiredService<ILogger<RefreshAdrsJobHandler>>());

            var response = await handler.Handle(job, CancellationToken.None).ConfigureAwait(false);
            response.Should().Be(JobResult.OperationSucceeded);
        }
    }
}
