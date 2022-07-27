using System;
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

            var applications = await UpdateAndReturnApplications(request.TenantId).ConfigureAwait(false);

            int totalUpdates = applications.Count(x => x.WasUpdated);

            foreach (var applicationName in applications.Select(x => x.ApplicationName))
            {
                totalUpdates += await UpdateArchitectureDecisionRecords(request.TenantId, applicationName).ConfigureAwait(false);
            }

            logger.LogInformation("END {JobName} [Elapsed] - {TotalUpdates} records were updated", nameof(RefreshAdrsJob), timer.Elapsed.ToString(), totalUpdates);

            if (totalUpdates > 0)
            {
                return JobResult.OperationSucceeded;
            }

            return JobResult.NoWorkPerformed;
        }

        private async Task<IEnumerable<(string ApplicationName, bool WasUpdated)>> UpdateAndReturnApplications(string tenantId)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(UpdateAndReturnApplications), tenantId);

            var results = new List<(string ApplicationName, bool WasUpdated)>(256);

            var repositories = await gitHubApi.GetRepositories(tenantId).ConfigureAwait(false);

            foreach (var repository in repositories)
            {
                bool wasUpdated = false;

                var key = string.Join("/", tenantId, repository.ApplicationName);

                var exists = await applicationRepository.Exists(key).ConfigureAwait(false);

                if (!exists)
                {
                    var aggregate = new Domain.Model.Application(
                        tenantId,
                        repository.ApplicationName!,
                        repository.ApplicationUrl!,
                        repository.ApplicationDescription);

                    await applicationRepository.Save(aggregate).ConfigureAwait(false);
                    wasUpdated = true;
                }

                results.Add((ApplicationName: repository.ApplicationName, WasUpdated: wasUpdated));
            }

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(UpdateAndReturnApplications), tenantId, timer.Elapsed.ToString());

            return results;
        }

        private async Task<int> UpdateArchitectureDecisionRecords(string tenantId, string applicationName)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(UpdateArchitectureDecisionRecords), string.Join(',', tenantId, applicationName));

            var totalUpdates = 0;
            var apiResults = await gitHubApi.FindArchitectureDecisionRecords(tenantId, applicationName).ConfigureAwait(false);

            foreach (var apiResult in apiResults)
            {
                var key = string.Join('/', tenantId, apiResult.ApplicationName, apiResult.FilePath);

                var adr = await adrRepository.Read(key).ConfigureAwait(false)
                    ?? new ArchitectureDecisionRecord(
                        tenantId,
                        apiResult.ApplicationName,
                        apiResult.FilePath,
                        null,
                        null,
                        null);

                var content = await gitHubApi.DownloadArchitectureDecisionRecord(
                    tenantId,
                    apiResult.ApplicationName,
                    apiResult.FilePath,
                    FileFormat.Raw).ConfigureAwait(false);

                adr.UpdateContent(content);

                await adrRepository.Save(adr).ConfigureAwait(false);

                totalUpdates++;

                // GitHub doesn't like rapid-fire requests
                await Task.Delay(TimeSpan.FromSeconds(1.5)).ConfigureAwait(false);
            }

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(UpdateArchitectureDecisionRecords), string.Join(',', tenantId, applicationName), timer.Elapsed.ToString());

            return totalUpdates;
        }
    }
}
