using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;

public interface IJobRepeatedInDay
{
    List<JobDay> Days { get; }
}

public class JobDay
{
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Day { get; set; }

    public bool IsValid => Hour is >= 0 and < 24 &&
                           Minute is >= 0 and < 60 &&
                           Day is > 0 and <= 31;
}

public class JobRepeatedInDayProcessorFactory : IJobProcessorFactory
{
    public IJobProcessor CreateInstance(JobConfig jobConfig)
    {
        return new JobRepeatedInDayProcessor
               {
                   Days = jobConfig.Days,
               };
    }
}

public class JobRepeatedInDayProcessor : JobProcessorBase, IJobRepeatedInDay
{
    public List<JobDay> Days { get; init; } = new();

    public override string LogParameters => $"days {string.Join(" / ", Days.Select(t => $"{t.Day} - {t.Hour}:{t.Minute}"))}";

    public override bool CanStartFirstTimeImmediately => false;

    public override DateTime GetNextStartDate(IDateTimeProvider dateTimeProvider)
    {
        var now = dateTimeProvider.UtcNow;
        var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
        var next = Days.OrderBy(x => x.Day)
                       .ThenBy(x => x.Hour)
                       .ThenBy(x => x.Minute)
                       .FirstOrDefault(x => x.IsValid &&
                                            x.Day <= daysInMonth &&
                                            (x.Day > now.Day ||
                                             x.Day == now.Day &&
                                             (x.Hour > now.Hour ||
                                              x.Hour == now.Hour && x.Minute > now.Minute)));
        if (next != null)
            return new DateTime(now.Year, now.Month, next.Day, next.Hour, next.Minute, 0);

        for (var i = 0; i < 12; i++)
        {
            var date = now.Date.AddMonths(1);
            daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            next = Days.OrderBy(x => x.Day)
                       .ThenBy(x => x.Hour)
                       .ThenBy(x => x.Minute)
                       .FirstOrDefault(x => x.IsValid &&
                                            x.Day <= daysInMonth);
            if (next != null)
            {
                return new DateTime(date.Year, date.Month, next.Day, next.Hour, next.Minute, 0);
            }
        }

        return DateTime.MaxValue;
    }
}