using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;

public interface IJobRepeated
{
    int IntervalInSeconds { get; }
    int DelayBeforeStartInSeconds { get; }
}

public class JobRepeatedProcessorFactory : IJobProcessorFactory
{
    public IJobProcessor CreateInstance(JobConfig jobConfig)
    {
        return new JobRepeatedProcessor
               {
                   IntervalInSeconds = jobConfig.IntervalInSeconds,
                   DelayBeforeStartInSeconds = jobConfig.DelayBeforeStartInSeconds,
               };
    }
}

public class JobRepeatedProcessor : JobProcessorBase, IJobRepeated
{
    public int IntervalInSeconds { get; init; }
    public int DelayBeforeStartInSeconds { get; init; }

    private TimeSpan interval => TimeSpan.FromSeconds(Math.Max(IntervalInSeconds, 1));
    private TimeSpan delayBeforeStart => TimeSpan.FromSeconds(Math.Max(DelayBeforeStartInSeconds, 0));

    public override string LogParameters => $"interval {interval}, delay {delayBeforeStart}";

    public override TimeSpan GetDelayBeforeStart() => delayBeforeStart;

    public override DateTime GetNextStartDate(IDateTimeProvider dateTimeProvider) => dateTimeProvider.UtcNow.Add(interval);
}