using MegaDTelegramRemoteControl.Services.Interfaces;
using System;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices;

public class JobSchedulerManagerBase : IJobSchedulerManager
{
    public void ModifyJobNextStart(string jobName, TimeSpan delayBeforeStart, bool modifyOnlyIfNewDateEarlier = true)
    {
        OnModifyJobNextStart(jobName, delayBeforeStart, modifyOnlyIfNewDateEarlier);
    }

    public event Action<string, TimeSpan, bool> OnModifyJobNextStart = (_, _, _) => { };
}