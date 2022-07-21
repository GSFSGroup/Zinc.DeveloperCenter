using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using RedLine.Domain;
using Zinc.DeveloperCenter.Host.Jobs.Configuration;
using Zinc.DeveloperCenter.Host.Jobs.HealthChecks;

namespace Zinc.DeveloperCenter.Host.Jobs.RefreshAdrs
{
    /// <summary>
    /// A Quartz job used to refresh the database with ADR details stored in the GSFSGroup GitHub repository.
    /// </summary>
    [DisallowConcurrentExecution]
    internal class RefreshGSFSGroupAdrsJob : IJob
    {
        internal static readonly string SectionName = $"Jobs:{nameof(RefreshGSFSGroupAdrsJob)}";
        private readonly IMediator mediator;
        private readonly string tenantId = "GSFSGroup";
        private readonly Guid correlationId;
        private readonly ILogger<RefreshGSFSGroupAdrsJob> logger;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="mediator">The <see cref="IMediator"/> used to send the <see cref="RefreshAdrsJob"/>.</param>
        /// <param name="correlationId">The <see cref="ICorrelationId"/> for the request.</param>
        /// <param name="logger">A diagnostic logger.</param>
        public RefreshGSFSGroupAdrsJob(
            IMediator mediator,
            ICorrelationId correlationId,
            ILogger<RefreshGSFSGroupAdrsJob> logger)
        {
            this.mediator = mediator;
            this.correlationId = correlationId.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Executes the job.
        /// </summary>
        /// <param name="context">The <see cref="IJobExecutionContext"/> provided by Quartz.</param>
        /// <returns>A <see cref="Task"/> used to wait the operation.</returns>
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var activity = new RefreshGSFSGroupAdrsJob(tenantId, correlationId);

                await mediator.Send(activity).ConfigureAwait(false);

                JobHealthCheck<RefreshGSFSGroupAdrsJob>.Heartbeat();
            }
            catch (Exception e)
            {
                /* NOTE:
                 * The Quartz documentation recommends NOT throwing exceptions from jobs, because the job will
                 * just get executed again immediately, and will likely throw the same exception. So, following
                 * their best practice guidelines, we swallow the exception and let the job execute at its next
                 * scheduled time. Note also that the JobHealthCheck<OutboxJob>.Heartbeat() call will not be made,
                 * so we will know the job is not executing when our health check alarm bells start going off.
                 * */
                logger.LogError(
                    e,
                    "{Error} executing {Job}: {Message}",
                    e.GetType().Name,
                    GetType().FullName,
                    e.Message);
            }
        }

        /// <summary>
        /// Configures the job.
        /// </summary>
        /// <param name="quartz">The <see cref="IServiceCollectionQuartzConfigurator"/> used to configure the job.</param>
        /// <param name="configuration">The application configuration settings.</param>
        internal static void ConfigureJob(IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration)
        {
            var jobConfig = configuration
                .GetSection(SectionName)
                .Get<JobConfig>();

            if (jobConfig.Disabled || jobConfig.CronSchedule == null)
            {
                return;
            }

            quartz.ScheduleJob<RefreshGSFSGroupAdrsJob>(
                trigger => trigger
                    .WithIdentity($"{nameof(RefreshGSFSGroupAdrsJob)}Trigger")
                    .StartAt(DateTimeOffset.UtcNow.AddSeconds(15))
                    .WithCronSchedule(jobConfig.CronSchedule)
                    .WithDescription($"A cron-based trigger for the {nameof(RefreshGSFSGroupAdrsJob)}."),
                job => job
                    .WithIdentity(nameof(RefreshGSFSGroupAdrsJob))
                    .WithDescription("The job used to refresh the database with ADR details stored in the GSFSGroup GitHub repository."));
        }

        /// <summary>
        /// Configures the job health check.
        /// </summary>
        /// <param name="healthChecks">The <see cref="IHealthChecksBuilder"/> used to configure the health check.</param>
        /// <param name="configuration">The application configuration settings.</param>
        internal static void ConfigureHealthCheck(IHealthChecksBuilder healthChecks, IConfiguration configuration)
        {
            var jobConfig = configuration
                .GetSection(SectionName)
                .Get<JobConfig>();

            if (jobConfig.Disabled)
            {
                return;
            }

            healthChecks.AddAsyncCheck(
                typeof(RefreshGSFSGroupAdrsJob).FullName ?? throw new NullReferenceException(),
                () => new JobHealthCheck<RefreshGSFSGroupAdrsJob>(
                    jobConfig.DegradedThreshold,
                    jobConfig.UnhealthyThreshold).CheckAsync());
        }
    }
}
