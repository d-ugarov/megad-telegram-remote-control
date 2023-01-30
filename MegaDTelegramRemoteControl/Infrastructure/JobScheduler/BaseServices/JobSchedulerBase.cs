using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;
using MegaDTelegramRemoteControl.Infrastructure.JobScheduler.Models;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices;

public abstract class JobSchedulerBase : IHostedService, IDisposable
{
    private readonly IServiceProvider services;
    private readonly JobSchedulerConfig configuration;
    private readonly IJobSchedulerManager manager;
    protected readonly ILogger<JobSchedulerBase> logger;

    private readonly Dictionary<JobMode, IJobProcessorFactory> jobProcessorFactories;

    private readonly Dictionary<string, CurrentJob> activeJobs;
    private volatile bool canWork;

    private readonly Guid instanceId;

    protected JobSchedulerBase(IServiceProvider services,
        JobSchedulerConfig configuration,
        ILogger<JobSchedulerBase> logger,
        Guid? instanceId = null,
        IJobSchedulerManager? manager = null)
    {
        this.services = services;
        this.configuration = configuration;
        this.logger = logger;

        this.manager = manager ?? new JobSchedulerManagerBase();
        this.manager.OnModifyJobNextStart += ModifyJobNextStart;

        jobProcessorFactories = new()
                                {
                                    {JobMode.OneTime, new JobOneTimeProcessorFactory()},
                                    {JobMode.Repeated, new JobRepeatedProcessorFactory()},
                                    {JobMode.RepeatedInDays, new JobRepeatedInDayProcessorFactory()},
                                    {JobMode.RepeatedInMinutes, new JobRepeatedInMinutesProcessorFactory()},
                                    {JobMode.RepeatedInTime, new JobRepeatedInTimeProcessorFactory()},
                                    {JobMode.RepeatedInWeekdays, new JobRepeatedInWeekdayProcessorFactory()},
                                };

        activeJobs = new();
        this.instanceId = instanceId ?? Guid.NewGuid();
    }

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            await WarmUpCacheAsync(true);

            canWork = true;

            foreach (var job in configuration.Jobs)
            {
                if (!job.IsActive)
                {
                    logger.LogTrace($"[JobScheduler] Job '{job.Name}' disabled");
                    continue;
                }

                if (!jobProcessorFactories.TryGetValue(job.Mode, out var jobFactory))
                {
                    logger.LogTrace($"[JobScheduler] Job '{job.Name}' has no factory for mode '{job.Mode}'");
                    continue;
                }

                var currentJob = new CurrentJob(job, jobFactory.CreateInstance(job), new DateTimeProvider());

                logger.LogInformation($"[JobScheduler] Job '{currentJob.Name}' started (mode {job.Mode}, {currentJob.LogParameters}, " +
                                      $"repeatedly {currentJob.CanWorkRepeatedly}, " +
                                      $"first start immediately {currentJob.CanStartFirstTimeImmediately})");

                activeJobs[job.Name] = currentJob;

                await Task.Factory.StartNew(async () => await StartSchedulerJobAsync(currentJob), CancellationToken.None);
            }

            if (!activeJobs.Any())
                logger.LogInformation("[JobScheduler] Jobs are disabled");

            await Task.Factory.StartNew(async () => await WarmUpCacheAsync(false), new CancellationToken());
        });
    }

    public virtual Task StopAsync(CancellationToken cancellationToken)
    {
        Dispose();
        return Task.CompletedTask;
    }

    public virtual void Dispose()
    {
        canWork = false;
    }

    protected virtual async Task WarmUpCacheAsync(bool initPrimaryCache)
    {
        using var scope = services.CreateScope();

        var cacheServices = scope.ServiceProvider.GetRequiredService<IEnumerable<IWarmupCacheService>>().ToList();

        var cacheType = initPrimaryCache ? "primary" : "secondary";
        logger.LogInformation($"[JobScheduler] WarmingUp {cacheType} cache starting");

        var sw = new Stopwatch();

        foreach (var service in cacheServices)
        {
            logger.LogInformation($"[WarmUp:{instanceId}] {service.GetType().Name} started");

            sw.Restart();

            await service.InitCacheAsync(initPrimaryCache);

            sw.Stop();

            logger.LogInformation($"[WarmUp:{instanceId}] {service.GetType().Name} done ({sw.Elapsed.TotalSeconds:0.####} sec)");
        }

        logger.LogInformation($"[JobScheduler] WarmingUp {cacheType} cache done");
    }

    private async Task StartSchedulerJobAsync(CurrentJob job)
    {
        await Task.Delay(job.DelayBeforeStart, CancellationToken.None);

        var waitNextRunDate = !job.CanStartFirstTimeImmediately;

        while (canWork)
        {
            if (waitNextRunDate)
            {
                while (!job.CanStart)
                {
                    await Task.Delay(500);

                    if (!canWork)
                        return;
                }
            }

            var jobStatus = await ProcessJobAsync(job);

            if (jobStatus == ProcessJobStatus.JobNotFound)
            {
                logger.LogError($"[JobScheduler] Unknown job '{job.Name}' stopped");
                return;
            }

            if (!job.CanWorkRepeatedly)
            {
                logger.LogInformation($"[JobScheduler] Job '{job.Name}' finished");
                return;
            }

            job.UpdateNextRun();
            waitNextRunDate = true;

            VerboseLogging($"Job '{job.Name}' next run date: {job.NextStart.ToDateString()}");
        }
    }

    private async Task<ProcessJobStatus> ProcessJobAsync(CurrentJob job)
    {
        var sw = Stopwatch.StartNew();
        VerboseLogging($"Job '{job.Name}' started");

        var result = await InvokeOperations.InvokeOperationAsync(async () =>
        {
            using var scope = services.CreateScope();
            return await ProcessJobAsync(job.Name, scope);
        });

        sw.Stop();
        VerboseLogging($"Job '{job.Name}' done ({sw.Elapsed.TotalSeconds:0.####} sec): {result.Report()}");

        return result.IsSuccess ? result.Data : default;
    }

    protected abstract Task<ProcessJobStatus> ProcessJobAsync(string jobName, IServiceScope scope);

    protected virtual void ModifyJobNextStart(string jobName, TimeSpan delayBeforeStart,
        bool modifyOnlyIfNewDateEarlier = true)
    {
        if (!activeJobs.TryGetValue(jobName, out var job))
        {
            logger.LogError($"[JobScheduler] Try modify unknown job '{jobName}'");
            return;
        }

        var nextStart = DateTime.UtcNow.Add(delayBeforeStart);

        if (modifyOnlyIfNewDateEarlier && job.NextStart < nextStart)
            return;

        job.UpdateNextRun(nextStart);

        VerboseLogging($"Job '{job.Name}' next run date modified: {job.NextStart.ToDateString()}");
    }

    private void VerboseLogging(string text)
    {
        if (!configuration.VerboseLogging)
            return;

        logger.LogTrace($"[JobScheduler] {text}");
    }

    protected enum ProcessJobStatus
    {
        Ok,
        JobNotFound,
    }
}