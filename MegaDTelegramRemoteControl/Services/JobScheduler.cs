using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services;

public class JobScheduler : JobSchedulerBase
{
    public JobScheduler(IServiceProvider services, IOptions<JobSchedulerConfig> configuration, ILogger<JobScheduler> logger)
        : base(services, configuration.Value, logger, Constants.InstanceId)
    {
    }

    protected override Task<ProcessJobStatus> ProcessJobAsync(string jobName, IServiceScope scope)
    {
        var processStatus = ProcessJobStatus.Ok;

        switch (jobName)
        {
            default:
                processStatus = ProcessJobStatus.JobNotFound;
                break;
        }

        return Task.FromResult(processStatus);
    }
}