using System;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IJobSchedulerManager
{
    void ModifyJobNextStart(string jobName, TimeSpan delayBeforeStart, bool modifyOnlyIfNewDateEarlier = true);

    event Action<string, TimeSpan, bool> OnModifyJobNextStart;
}