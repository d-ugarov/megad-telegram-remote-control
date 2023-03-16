using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;

public interface IJobRepeatedInWeekday
{
    List<JobWeekday> Weekdays { get; }
}

public class JobWeekday
{
    public int Hour { get; set; }
    public int Minute { get; set; }
    public DayOfWeek Weekday { get; set; }

    public bool IsValid => Hour is >= 0 and < 24 &&
                           Minute is >= 0 and < 60;
}

public class JobRepeatedInWeekdayProcessorFactory : IJobProcessorFactory
{
    public IJobProcessor CreateInstance(JobConfig jobConfig)
    {
        return new JobRepeatedInWeekdayProcessor
               {
                   Weekdays = jobConfig.Weekdays,
               };
    }
}

public class JobRepeatedInWeekdayProcessor : JobProcessorBase, IJobRepeatedInWeekday
{
    public List<JobWeekday> Weekdays { get; init; } = new();

    public override string LogParameters => $"weekdays {string.Join(" / ", Weekdays.Select(t => $"{t.Weekday} - {t.Hour}:{t.Minute}"))}";

    public override bool CanStartFirstTimeImmediately => false;

    public override DateTime GetNextStartDate(IDateTimeProvider dateTimeProvider)
    {
        var now = dateTimeProvider.UtcNow;
        var next = Weekdays.OrderBy(x => x.Weekday)
                           .ThenBy(x => x.Hour)
                           .ThenBy(x => x.Minute)
                           .FirstOrDefault(x => x.IsValid &&
                                                (x.Weekday > now.DayOfWeek ||
                                                 x.Weekday == now.DayOfWeek &&
                                                 (x.Hour > now.Hour ||
                                                  x.Hour == now.Hour && x.Minute > now.Minute)));
        if (next != null)
            return new DateTime(now.Year, now.Month, now.Day, next.Hour, next.Minute, 0).AddDays(next.Weekday - now.DayOfWeek);

        var date = now.Date.AddDays(7 - (int)now.DayOfWeek);

        next = Weekdays.OrderBy(x => x.Weekday)
                       .ThenBy(x => x.Hour)
                       .ThenBy(x => x.Minute)
                       .FirstOrDefault(x => x.IsValid);
        if (next != null)
        {
            return new DateTime(date.Year, date.Month, date.Day, next.Hour, next.Minute, 0).AddDays(next.Weekday - date.DayOfWeek);
        }

        return DateTime.MaxValue;
    }
}