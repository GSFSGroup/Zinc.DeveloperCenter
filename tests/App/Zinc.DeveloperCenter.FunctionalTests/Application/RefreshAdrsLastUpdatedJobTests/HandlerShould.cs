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
using Zinc.DeveloperCenter.Application.Jobs.RefreshAdrsLastUpdated;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Services.GitHub;
#pragma warning restore S1128 // Unused "using" should be removed

namespace Zinc.DeveloperCenter.FunctionalTests.Application.RefreshAdrsLastUpdatedJobTests
{
    public class HandlerShould : FunctionalTestBase
    {
        public HandlerShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        // [Fact(Skip = "Long running test.")]
        [Fact]
        public async Task UpdateTheDatabase()
        {
            var job = new RefreshAdrsLastUpdatedJob("GSFSGroup", Guid.NewGuid());

            var handler = new RefreshAdrsLastUpdatedJobHandler(
                GetRequiredService<IGitHubApiService>(),
                GetRequiredService<IArchitectureDecisionRecordRepository>(),
                GetRequiredService<ILogger<RefreshAdrsLastUpdatedJobHandler>>());

            var response = await handler.Handle(job, CancellationToken.None).ConfigureAwait(false);
            response.Should().Be(JobResult.OperationSucceeded);

            var connection = GetRequiredService<IDbConnection>();

            var totalAdrs = await connection
                .ExecuteScalarAsync<int>("select count(*) from developercenter.architecture_decision_record where updated_on is not null;")
                .ConfigureAwait(false);

            Output.WriteLine($"!!!!!!!!!!{totalAdrs} ADRs were updated!!!!!!!!!!");

            totalAdrs.Should().BeGreaterThan(20);
        }
    }
}
