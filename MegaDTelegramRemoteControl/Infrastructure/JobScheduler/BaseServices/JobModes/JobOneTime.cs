using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using System;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;

public interface IJobOneTime
{
    int DelayBeforeStartInSeconds { get; }
}

public class JobOneTimeProcessorFactory : IJobProcessorFactory
{
    public IJobProcessor CreateInstance(JobConfig jobConfig)
    {
        return new JobOneTimeProcessor
               {
                   DelayBeforeStartInSeconds = jobConfig.DelayBeforeStartInSeconds,
               };
    }
}

public class JobOneTimeProcessor : JobProcessorBase, IJobOneTime
{
    public int DelayBeforeStartInSeconds { get; init; }

    private TimeSpan delayBeforeStart => TimeSpan.FromSeconds(Math.Max(DelayBeforeStartInSeconds, 0));

    public override string LogParameters => $"delay {delayBeforeStart}";

    public override bool CanWorkRepeatedly => false;

    public override TimeSpan GetDelayBeforeStart() => delayBeforeStart;
}