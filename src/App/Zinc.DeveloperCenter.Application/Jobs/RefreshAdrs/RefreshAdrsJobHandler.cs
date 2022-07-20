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
        private readonly IApplicationRepository applicationRepository;
        private readonly IArchitectureDecisionRecordRepository adrRepository;
        private readonly ILogger<RefreshAdrsJobHandler> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="gitHubApi">The <see cref="IGitHubApiService"/>.</param>
        /// <param name="applicationRepository">The <see cref="IApplicationRepository"/>.</param>
        /// <param name="adrRepository">The <see cref="IArchitectureDecisionRecordRepository"/>.</param>
        /// <param name="logger">A diagnostic logger.</param>
        public RefreshAdrsJobHandler(
            IGitHubApiService gitHubApi,
            IApplicationRepository applicationRepository,
            IArchitectureDecisionRecordRepository adrRepository,
            ILogger<RefreshAdrsJobHandler> logger)
        {
            this.gitHubApi = gitHubApi;
            this.applicationRepository = applicationRepository;
            this.adrRepository = adrRepository;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public override async Task<JobResult> Handle(RefreshAdrsJob request, CancellationToken cancellationToken)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogInformation("BEGIN {JobName}...", nameof(RefreshAdrsJob));

            var applications = await UpdateAndReturnApplications().ConfigureAwait(false);

            int totalUpdates = applications.Count(x => x.WasUpdated);

            foreach (var applicationName in applications.Select(x => x.ApplicationName))
            {
                totalUpdates += await UpdateArchitectureDecisionRecords(applicationName).ConfigureAwait(false);
            }

            logger.LogInformation("END {JobName} [Elapsed] - {TotalUpdates} records were updated", nameof(RefreshAdrsJob), timer.Elapsed.ToString(), totalUpdates);

            if (totalUpdates > 0)
            {
                return JobResult.OperationSucceeded;
            }

            return JobResult.NoWorkPerformed;
        }

        private async Task<IEnumerable<(string ApplicationName, bool WasUpdated)>> UpdateAndReturnApplications()
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(UpdateAndReturnApplications), string.Empty);

            var results = new List<(string ApplicationName, bool WasUpdated)>(256);

            var repositories = await gitHubApi.GetRepositories().ConfigureAwait(false);

            foreach (var applicationName in repositories.Select(x => x.ApplicationName))
            {
                bool wasUpdated = false;
                var exists = await applicationRepository.Exists(applicationName).ConfigureAwait(false);

                if (!exists)
                {
                    await applicationRepository.Save(new Domain.Model.Application(applicationName!)).ConfigureAwait(false);
                    wasUpdated = true;
                }

                results.Add((ApplicationName: applicationName!, WasUpdated: wasUpdated));
            }

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(UpdateAndReturnApplications), string.Empty, timer.Elapsed.ToString());

            return results;
        }

        private async Task<int> UpdateArchitectureDecisionRecords(string applicationName)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(UpdateArchitectureDecisionRecords), applicationName);

            var totalUpdates = 0;
            var apiResults = await gitHubApi.GetArchitectureDecisionRecords(applicationName).ConfigureAwait(false);

            foreach (var apiResult in apiResults)
            {
                var adr = await adrRepository.Read($"{apiResult.ApplicationName}/{apiResult.Number}").ConfigureAwait(false);

                if (adr == null)
                {
                    var content = await gitHubApi.DownloadArchitectureDecisionRecord(apiResult.DownloadUrl!).ConfigureAwait(false);

                    adr = new ArchitectureDecisionRecord(
                        apiResult.ApplicationName!,
                        apiResult.Number,
                        apiResult.Title!,
                        apiResult.LastUpdated!,
                        apiResult.DownloadUrl!,
                        apiResult.HtmlUrl!,
                        content);

                    await adrRepository.Save(adr).ConfigureAwait(false);
                    totalUpdates++;
                }
                else if (adr.LastUpdated != apiResult.LastUpdated)
                {
                    var content = await gitHubApi.DownloadArchitectureDecisionRecord(apiResult.DownloadUrl!).ConfigureAwait(false);

                    adr.UpdateContent(content);
                    adr.UpdateDownloadUrl(apiResult.DownloadUrl!);
                    adr.UpdateHtmlUrl(apiResult.HtmlUrl!);
                    adr.UpdateLastUpdated(apiResult.LastUpdated!);
                    adr.UpdateTitle(apiResult.Title!);

                    await adrRepository.Save(adr).ConfigureAwait(false);
                    totalUpdates++;
                }
            }

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(UpdateArchitectureDecisionRecords), applicationName, timer.Elapsed.ToString());

            return totalUpdates;
        }
    }
}
