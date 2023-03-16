using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;

public interface IJobRepeatedInTime
{
    List<JobTime> Times { get; }
}

public class JobTime
{
    public int Hour { get; set; }
    public int Minute { get; set; }

    public bool IsValid => Hour is >= 0 and < 24 &&
                           Minute is >= 0 and < 60;
}

public class JobRepeatedInTimeProcessorFactory : IJobProcessorFactory
{
    public IJobProcessor CreateInstance(JobConfig jobConfig)
    {
        return new JobRepeatedInTimeProcessor
               {
                   Times = jobConfig.Times,
               };
    }
}

public class JobRepeatedInTimeProcessor : JobProcessorBase, IJobRepeatedInTime
{
    public List<JobTime> Times { get; init; } = new();

    public override string LogParameters => $"times {string.Join(" / ", Times.Select(t => $"{t.Hour}:{t.Minute}"))}";

    public override bool CanStartFirstTimeImmediately => false;

    public override DateTime GetNextStartDate(IDateTimeProvider dateTimeProvider)
    {
        var now = dateTimeProvider.UtcNow;
        var next = Times.OrderBy(x => x.Hour)
                        .ThenBy(x => x.Minute)
                        .FirstOrDefault(x => x.IsValid &&
                                             (x.Hour > now.Hour ||
                                              x.Hour == now.Hour && x.Minute > now.Minute));
        if (next != null)
            return new DateTime(now.Year, now.Month, now.Day, next.Hour, next.Minute, 0);

        next = Times.OrderBy(x => x.Hour)
                    .ThenBy(x => x.Minute)
                    .FirstOrDefault(x => x.IsValid);
        if (next != null)
        {
            var date = now.Date.AddDays(1);
            return new DateTime(date.Year, date.Month, date.Day, next.Hour, next.Minute, 0);
        }

        return DateTime.MaxValue;
    }
}