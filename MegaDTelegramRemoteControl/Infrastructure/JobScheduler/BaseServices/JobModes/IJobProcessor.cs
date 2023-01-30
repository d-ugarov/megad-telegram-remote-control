using MegaDTelegramRemoteControl.Services.Interfaces;
using System;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;

public interface IJobProcessor
{
    string LogParameters { get; }

    bool CanWorkRepeatedly { get; }

    bool CanStartFirstTimeImmediately { get; }

    TimeSpan GetDelayBeforeStart();

    DateTime GetNextStartDate(IDateTimeProvider dateTimeProvider);
}

public abstract class JobProcessorBase : IJobProcessor
{
    public virtual string LogParameters => string.Empty;

    public virtual bool CanWorkRepeatedly => true;
    
    public virtual bool CanStartFirstTimeImmediately => true;

    public virtual TimeSpan GetDelayBeforeStart() => TimeSpan.Zero;

    public virtual DateTime GetNextStartDate(IDateTimeProvider dateTimeProvider) => DateTime.MaxValue;
}