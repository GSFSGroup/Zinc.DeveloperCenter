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

            var totalUpdates = await UpdateApplications(request.TenantId).ConfigureAwait(false);

            totalUpdates += await UpdateArchitectureDecisionRecords(request.TenantId).ConfigureAwait(false);

            logger.LogInformation("END {JobName} [Elapsed] - {TotalUpdates} records were updated", nameof(RefreshAdrsJob), timer.Elapsed.ToString(), totalUpdates);

            if (totalUpdates > 0)
            {
                return JobResult.OperationSucceeded;
            }

            return JobResult.NoWorkPerformed;
        }

        private async Task<int> UpdateApplications(string tenantId)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(UpdateApplications), tenantId);

            var totalUpdates = 0;

            var apiResults = await gitHubApi.GetRepositories(tenantId).ConfigureAwait(false);

            foreach (var apiResult in apiResults)
            {
                var key = string.Join("/", tenantId, apiResult.ApplicationName);

                var app = await applicationRepository.Read(key).ConfigureAwait(false);

                if (app == null)
                {
                    app = new Domain.Model.Application(
                        tenantId,
                        apiResult.ApplicationName!,
                        apiResult.ApplicationUrl!,
                        apiResult.ApplicationDescription);

                    await applicationRepository.Save(app).ConfigureAwait(false);
                    totalUpdates++;
                }
            }

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(UpdateApplications), tenantId, timer.Elapsed.ToString());

            return totalUpdates;
        }

        private async Task<int> UpdateArchitectureDecisionRecords(string tenantId)
        {
            Stopwatch timer = Stopwatch.StartNew();

            logger.LogDebug("BEGIN {MethodName}({Args})...", nameof(UpdateArchitectureDecisionRecords), tenantId);

            var totalUpdates = 0;

            var models = (await gitHubApi.FindArchitectureDecisionRecords(tenantId, "Zinc.Templates")
                .ConfigureAwait(false))
                .ToHashSet();

            // GitHub doesn't like rapid-fire requests
            await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

            var remainingModels = await FindArchitectureDecisionRecords(tenantId).ConfigureAwait(false);

            models.UnionWith(remainingModels);

            foreach (var model in models)
            {
                // GitHub doesn't like rapid-fire requests
                await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

                var key = string.Join('/', tenantId, model.ApplicationName, model.FilePath);

                var adr = await adrRepository.Read(key).ConfigureAwait(false)
                    ?? new ArchitectureDecisionRecord(
                        tenantId,
                        model.ApplicationName,
                        model.FilePath,
                        null,
                        null,
                        null);

                var contentModel = await gitHubApi.DownloadArchitectureDecisionRecord(
                    tenantId,
                    model.ApplicationName,
                    model.FilePath,
                    FileFormat.Raw).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(contentModel.Content))
                {
                    adr.UpdateContent(contentModel.Content);
                    await adrRepository.Save(adr).ConfigureAwait(false);
                    totalUpdates++;
                }
            }

            logger.LogDebug("END {MethodName}({Args}) [Elapsed]", nameof(UpdateArchitectureDecisionRecords), tenantId, timer.Elapsed.ToString());

            return totalUpdates;
        }

        private async Task<HashSet<GitHubArchitectureDecisionRecordModel>> FindArchitectureDecisionRecords(string tenantId)
        {
            int page = 1;
            int pageSize = 100;

            var results = new HashSet<GitHubArchitectureDecisionRecordModel>(256);

            var apiResults = (await gitHubApi.FindArchitectureDecisionRecords(tenantId, page, pageSize)
                .ConfigureAwait(false))
                .ToList();

            while (apiResults.Count > 0)
            {
                results.UnionWith(apiResults);

                page++;

                if (apiResults.Count == pageSize)
                {
                    // GitHub doesn't like rapid-fire requests
                    await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
                }
                else if (apiResults.Count < pageSize)
                {
                    break;
                }

                // GitHub doesn't like rapid-fire requests
                await Task.Delay(TimeSpan.FromSeconds(3)).ConfigureAwait(false);

                apiResults = (await gitHubApi.FindArchitectureDecisionRecords(tenantId, page, pageSize)
                    .ConfigureAwait(false))
                    .ToList();
            }

            return results;
        }
    }
}
