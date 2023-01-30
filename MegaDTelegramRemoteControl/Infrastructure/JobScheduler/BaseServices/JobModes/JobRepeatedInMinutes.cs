using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;

public interface IJobRepeatedInMinutes
{
    List<JobMinute> Minutes { get; }
}

public class JobMinute
{
    public int Minute { get; set; }

    public bool IsValid => Minute is >= 0 and < 60;
}

public class JobRepeatedInMinutesProcessorFactory : IJobProcessorFactory
{
    public IJobProcessor CreateInstance(JobConfig jobConfig)
    {
        return new JobRepeatedInMinutesProcessor
               {
                   Minutes = jobConfig.Minutes,
               };
    }
}

public class JobRepeatedInMinutesProcessor : JobProcessorBase, IJobRepeatedInMinutes
{
    public List<JobMinute> Minutes { get; init; } = new();

    public override string LogParameters => $"minutes {string.Join(" / ", Minutes.Select(t => $"{t.Minute}"))}";

    public override bool CanStartFirstTimeImmediately => false;

    public override DateTime GetNextStartDate(IDateTimeProvider dateTimeProvider)
    {
        var now = dateTimeProvider.UtcNow;
        var next = Minutes.OrderBy(x => x.Minute)
                          .FirstOrDefault(x => x.IsValid && x.Minute > now.Minute);
        if (next != null)
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, next.Minute, 0);

        next = Minutes.OrderBy(x => x.Minute)
                      .FirstOrDefault(x => x.IsValid);
        if (next != null)
        {
            var date = now.AddHours(1);
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, next.Minute, 0);
        }

        return DateTime.MaxValue;
    }
}