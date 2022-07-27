using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RedLine.Application.Jobs;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Services.GitHub;

namespace Zinc.DeveloperCenter.Application.Jobs.RefreshAdrsLastUpdated
{
    internal class RefreshAdrsLastUpdatedJobHandler : JobHandlerBase<RefreshAdrsLastUpdatedJob>
    {
        private readonly IGitHubApiService gitHubApi;
        private readonly IArchitectureDecisionRecordRepository repository;
        private readonly ILogger<RefreshAdrsLastUpdatedJobHandler> logger;

        public RefreshAdrsLastUpdatedJobHandler(
            IGitHubApiService gitHubApi,
            IArchitectureDecisionRecordRepository repository,
            ILogger<RefreshAdrsLastUpdatedJobHandler> logger)
        {
            this.gitHubApi = gitHubApi;
            this.repository = repository;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<JobResult> Handle(RefreshAdrsLastUpdatedJob request, CancellationToken cancellationToken)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogInformation("BEGIN {JobName}...", nameof(RefreshAdrsLastUpdatedJob));

            var adrs = await repository.Query(new GetArchitectureDecisionRecordsDataQuery(request.TenantId)).ConfigureAwait(false);
            var totalUpdates = 0;

            foreach (var adr in adrs.Items)
            {
                var lastUpdatedDetails = await gitHubApi.GetLastUpdatedDetails(request.TenantId, adr.ApplicationName, adr.FilePath).ConfigureAwait(false);

                if (lastUpdatedDetails == default)
                {
                    continue;
                }

                if (adr.LastUpdatedBy != lastUpdatedDetails.LastUpdatedBy || adr.LastUpdatedOn != lastUpdatedDetails.LastUpdatedOn)
                {
                    adr.UpdateLastUpdated(lastUpdatedDetails.LastUpdatedBy, lastUpdatedDetails.LastUpdatedOn);

                    await repository.Save(adr).ConfigureAwait(false);

                    totalUpdates++;
                }

                // GitHub doesn't like rapid-fire requests
                await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            logger.LogInformation("END {JobName} [Elapsed] - {TotalUpdates} records were updated", nameof(RefreshAdrsLastUpdatedJob), timer.Elapsed.ToString(), totalUpdates);

            if (totalUpdates > 0)
            {
                return JobResult.OperationSucceeded;
            }

            return JobResult.NoWorkPerformed;
        }
    }
}
