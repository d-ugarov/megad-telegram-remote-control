using MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;
using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public class JobSchedulerConfig
{
    public List<JobConfig> Jobs { get; set; } = new();

    public bool VerboseLogging { get; set; } = false;
}

public class JobConfig : IJobRepeated, IJobOneTime, IJobRepeatedInTime, IJobRepeatedInMinutes, IJobRepeatedInDay, IJobRepeatedInWeekday
{
    #region Common

    public string Name { get; set; } = "";

    public bool IsActive { get; set; }

    public JobMode Mode { get; set; }

    #endregion

    #region Parameters by mode

    public List<JobTime> Times { get; set; } = new();

    public List<JobMinute> Minutes { get; set; } = new();

    public List<JobDay> Days { get; set; } = new();

    public List<JobWeekday> Weekdays { get; set; } = new();

    public int IntervalInSeconds { get; set; }

    public int DelayBeforeStartInSeconds { get; set; }

    #endregion
}

public enum JobMode
{
    /// <summary>
    /// Start job every X seconds
    /// </summary>
    Repeated,

    /// <summary>
    /// Start job once
    /// </summary>
    OneTime,

    /// <summary>
    /// Start job in selected time
    /// </summary>
    RepeatedInTime,

    /// <summary>
    /// Start job in selected minutes
    /// </summary>
    RepeatedInMinutes,

    /// <summary>
    /// Start job in selected time and selected day
    /// </summary>
    RepeatedInDays,

    /// <summary>
    /// Start job in selected time and selected weekday
    /// </summary>
    RepeatedInWeekdays,
}