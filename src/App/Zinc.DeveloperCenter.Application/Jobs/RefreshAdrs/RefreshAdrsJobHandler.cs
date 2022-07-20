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
    /// <summary>
    /// A job used to refresh ADRs in the database.
    /// </summary>
    public class RefreshAdrsJobHandler : JobHandlerBase<RefreshAdrsJob>
    {
        private readonly IGitHubApiService gitHubApi;
        private readonly IArchitectureDecisionRecordRepository repository;
        private readonly ILogger<RefreshAdrsJobHandler> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="gitHubApi">The <see cref="IGitHubApiService"/>.</param>
        /// <param name="repository">The <see cref="IArchitectureDecisionRecordRepository"/>.</param>
        /// <param name="logger">A diagnostic logger.</param>
        public RefreshAdrsJobHandler(
            IGitHubApiService gitHubApi,
            IArchitectureDecisionRecordRepository repository,
            ILogger<RefreshAdrsJobHandler> logger)
        {
            this.gitHubApi = gitHubApi;
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
                var adrs = await GetArchitectureDecisionRecords(application.ApplicationName!).ConfigureAwait(false);

                if (adrs.Any())
                {
                    foreach (var adr in adrs)
                    {
                        aggregateRoots.Add(new ArchitectureDecisionRecord(
                            application.ApplicationElement!,
                            application.ApplicationName!,
                            application.ApplicationDisplayName!,
                            adr.Title!,
                            adr.Number,
                            adr.LastUpdated!,
                            adr.DownloadUrl!,
                            adr.HtmlUrl!,
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

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(GetRepositories), string.Empty);

            var results = await gitHubApi.GetRepositories().ConfigureAwait(false);

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(GetRepositories), string.Empty, timer.Elapsed.ToString());

            return results;
        }

        private async Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> GetArchitectureDecisionRecords(string applicationName)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(GetArchitectureDecisionRecords), applicationName);

            var results = await gitHubApi.GetArchitectureDecisionRecords(applicationName, true).ConfigureAwait(false);

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(GetArchitectureDecisionRecords), applicationName, timer.Elapsed.ToString());

            return results;
        }
    }
}
