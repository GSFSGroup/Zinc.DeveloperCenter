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
using Zinc.DeveloperCenter.Application.Jobs.RefreshAdrsLastUpdated;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Services.GitHub;
#pragma warning restore S1128 // Unused "using" should be removed

namespace Zinc.DeveloperCenter.FunctionalTests.Application.RefreshAdrsJobTests
{
    public class HandlerShould : FunctionalTestBase
    {
        public HandlerShould(FunctionalTestFixture fixture, ITestOutputHelper output)
            : base(fixture, output)
        {
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "By design.")]
        public async Task UpdateTheDatabaseAndLastUpdated()
        {
            var tenantId = "GSFSGroup";
            var correlationId = Guid.NewGuid();

            var response = await RunRefreshAdrsJob(tenantId, correlationId).ConfigureAwait(true);
            response.Should().Be(JobResult.OperationSucceeded);

            var connection = GetRequiredService<IDbConnection>();

            var totalRepos = await connection
                .ExecuteScalarAsync<int>("select count(*) from developercenter.application;")
                .ConfigureAwait(false);

            Output.WriteLine($"!!!!!!!!!!{totalRepos} repositories were found!!!!!!!!!!");

            var totalAdrs = await connection
                .ExecuteScalarAsync<int>("select count(*) from developercenter.architecture_decision_record;")
                .ConfigureAwait(false);

            Output.WriteLine($"!!!!!!!!!!{totalAdrs} ADRs were found!!!!!!!!!!");

            totalRepos.Should().BeGreaterThan(50);
            totalAdrs.Should().BeGreaterThan(20);

            /*
            response = await RunRefreshAdrsLastUpdatedJob(tenantId, correlationId).ConfigureAwait(false);
            response.Should().Be(JobResult.OperationSucceeded);

            totalAdrs = await connection
                .ExecuteScalarAsync<int>("select count(*) from developercenter.architecture_decision_record where updated_by is not null AND updated_on is not null;")
                .ConfigureAwait(false);

            Output.WriteLine($"!!!!!!!!!!{totalAdrs} ADRs were updated!!!!!!!!!!");

            totalAdrs.Should().BeGreaterThan(20);
             * */
        }

        private async Task<JobResult> RunRefreshAdrsJob(string tenantId, Guid correlationId)
        {
            var job = new RefreshAdrsJob(tenantId, correlationId);

            var handler = new RefreshAdrsJobHandler(
                GetRequiredService<IGitHubApiService>(),
                GetRequiredService<IApplicationRepository>(),
                GetRequiredService<IArchitectureDecisionRecordRepository>(),
                GetRequiredService<ILogger<RefreshAdrsJobHandler>>());

            return await handler.Handle(job, CancellationToken.None).ConfigureAwait(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "By design.")]
        private async Task<JobResult> RunRefreshAdrsLastUpdatedJob(string tenantId, Guid correlationId)
        {
            var job = new RefreshAdrsLastUpdatedJob(tenantId, correlationId);

            var handler = new RefreshAdrsLastUpdatedJobHandler(
                GetRequiredService<IGitHubApiService>(),
                GetRequiredService<IArchitectureDecisionRecordRepository>(),
                GetRequiredService<ILogger<RefreshAdrsLastUpdatedJobHandler>>());

            return await handler.Handle(job, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
