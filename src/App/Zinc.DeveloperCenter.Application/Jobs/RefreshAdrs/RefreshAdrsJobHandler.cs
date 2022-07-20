using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RedLine.Application.Jobs;
using Zinc.DeveloperCenter.Domain.Model;
using Zinc.DeveloperCenter.Domain.Repositories;
using Zinc.DeveloperCenter.Domain.Services.GitHub;

namespace Zinc.DeveloperCenter.Application.Jobs.RefreshAdrs
{
    internal class RefreshAdrsJobHandler : JobHandlerBase<RefreshAdrsJob>
    {
        private readonly IGitGubApiService gitHub;
        private readonly IArchitectureDecisionRecordRepository repository;
        private readonly ILogger<RefreshAdrsJobHandler> logger;

        public RefreshAdrsJobHandler(
            IGitGubApiService gitHub,
            IArchitectureDecisionRecordRepository repository,
            ILogger<RefreshAdrsJobHandler> logger)
        {
            this.gitHub = gitHub;
            this.repository = repository;
            this.logger = logger;
        }

        public override async Task<JobResult> Handle(RefreshAdrsJob request, CancellationToken cancellationToken)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogInformation("BEGIN {JobName}...", nameof(RefreshAdrsJob));

            var applications = await GetRepositories().ConfigureAwait(false);
            var aggregateRoots = new List<ArchitectureDecisionRecord>(256);

            foreach (var application in applications)
            {
                var adrs = await GetArchitectureDecisionRecords(application.ApplicationName ?? string.Empty).ConfigureAwait(false);

                if (adrs.Any())
                {
                    foreach (var adr in adrs)
                    {
                        aggregateRoots.Add(new ArchitectureDecisionRecord(
                            application.ApplicationElement ?? string.Empty,
                            application.ApplicationName ?? string.Empty,
                            application.ApplicationDisplayName ?? string.Empty,
                            adr.Title ?? string.Empty,
                            adr.Number,
                            adr.LastUpdated ?? string.Empty,
                            adr.DownloadUrl ?? string.Empty,
                            adr.Content));
                    }
                }
            }

            int totalUpdates = 0;

            foreach (var aggregate in aggregateRoots)
            {
                totalUpdates += await repository.Save(aggregate).ConfigureAwait(false);
            }

            logger.LogInformation("END {JobName} [Elapsed]", nameof(RefreshAdrsJob), timer.Elapsed.ToString());

            if (totalUpdates > 0)
            {
                return JobResult.OperationSucceeded;
            }

            return JobResult.NoWorkPerformed;
        }

        private async Task<IEnumerable<GitHubRepositoryModel>> GetRepositories()
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(GetRepositories));

            // TODO do the work

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(GetRepositories), timer.Elapsed.ToString());

            return await Task.FromResult(Enumerable.Empty<GitHubRepositoryModel>()).ConfigureAwait(false);
        }

        private async Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> GetArchitectureDecisionRecords(string applicationName)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(GetArchitectureDecisionRecords), applicationName);

            // TODO do the work
            // var results = gitGub.GetArchitectureDecisionRecords(applicationName, true)

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(GetArchitectureDecisionRecords), applicationName, timer.Elapsed.ToString());

            return await Task.FromResult(Enumerable.Empty<GitHubArchitectureDecisionRecordModel>()).ConfigureAwait(false);
        }
    }
}
