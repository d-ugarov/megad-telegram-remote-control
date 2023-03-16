using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.Models;

public class CurrentJob
{
    private readonly JobConfig jobConfig;
    private readonly IJobProcessor jobProcessor;
    private readonly IDateTimeProvider dateTimeProvider;

    public DateTime NextStart { get; private set; }

    public CurrentJob(JobConfig jobConfig, IJobProcessor jobProcessor, IDateTimeProvider dateTimeProvider)
    {
        this.jobConfig = jobConfig;
        this.dateTimeProvider = dateTimeProvider;
        this.jobProcessor = jobProcessor;

        UpdateNextRun();
    }

    public void UpdateNextRun()
    {
        NextStart = jobProcessor.GetNextStartDate(dateTimeProvider);
    }

    public void UpdateNextRun(DateTime date)
    {
        NextStart = date;
    }

    public string LogParameters => jobProcessor.LogParameters;

    public bool CanWorkRepeatedly => jobProcessor.CanWorkRepeatedly;

    public bool CanStartFirstTimeImmediately => jobProcessor.CanStartFirstTimeImmediately;

    public TimeSpan DelayBeforeStart => jobProcessor.GetDelayBeforeStart();

    public bool CanStart => dateTimeProvider.UtcNow >= NextStart;

    public string Name => jobConfig.Name;
}