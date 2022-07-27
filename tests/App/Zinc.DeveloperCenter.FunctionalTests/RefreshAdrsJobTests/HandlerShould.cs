#pragma warning disable S1128 // Unused "using" should be removed
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
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
            var job = new RefreshAdrsJob("GSFSGroup", Guid.NewGuid());

            var handler = new RefreshAdrsJobHandler(
                GetRequiredService<IGitHubApiService>(),
                GetRequiredService<IApplicationRepository>(),
                GetRequiredService<IArchitectureDecisionRecordRepository>(),
                GetRequiredService<ILogger<RefreshAdrsJobHandler>>());

            var response = await handler.Handle(job, CancellationToken.None).ConfigureAwait(false);
            response.Should().Be(JobResult.OperationSucceeded);

            var connection = GetRequiredService<IDbConnection>();

            var totalRepos = await connection
                .ExecuteScalarAsync<int>("select count(*) from developercenter.application")
                .ConfigureAwait(false);

            Output.WriteLine($"!!!!!!!!!!{totalRepos} repositories were found!!!!!!!!!!");

            var totalAdrs = await connection
                .ExecuteScalarAsync<int>("select count(*) from developercenter.architecture_decision_record")
                .ConfigureAwait(false);

            Output.WriteLine($"!!!!!!!!!!{totalAdrs} ADRs were found!!!!!!!!!!");

            totalRepos.Should().BeGreaterThan(50);
            totalAdrs.Should().BeGreaterThan(20);
        }
    }
}
