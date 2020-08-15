using MegaDTelegramRemoteControl.Infrastructure.Jobs.Enums;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MegaDTelegramRemoteControl.Infrastructure.Jobs.Configurations
{
    public class InternalSchedulerConfig
    {
        public List<JobType> Jobs { get; set; }
    }

    public class JobTypeAttribute : Attribute
    {
        public TimeSpan DueTime;
        public TimeSpan Interval;
        public TimeSpan Period;

        public JobTypeAttribute(int dueTimeSec, int intervalSec, int period = 60)
        {
            DueTime = TimeSpan.FromSeconds(dueTimeSec);
            Interval = TimeSpan.FromSeconds(intervalSec);
            Period = TimeSpan.FromSeconds(period);
        }
    }

    internal class Job
    {
        public Timer Timer { get; set; }
        public TimeSpan Interval { get; set; }
    }
}