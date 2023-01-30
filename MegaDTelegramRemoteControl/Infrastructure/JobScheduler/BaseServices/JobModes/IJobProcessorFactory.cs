using MegaDTelegramRemoteControl.Infrastructure.Configurations;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices.JobModes;

public interface IJobProcessorFactory
{
    IJobProcessor CreateInstance(JobConfig jobConfig);
}