using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices;
using MegaDTelegramRemoteControl.Services.Interfaces;
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

    protected override async Task<ProcessJobStatus> ProcessJobAsync(string jobName, IServiceScope scope)
    {
        var processStatus = ProcessJobStatus.Ok;

        switch (jobName)
        {
            case "UpdateHomeState":
                await UpdateHomeStateAsync(scope);
                break;
            default:
                processStatus = ProcessJobStatus.JobNotFound;
                break;
        }

        return processStatus;
    }

    private static async Task UpdateHomeStateAsync(IServiceScope scope)
    {
        var rateService = scope.ServiceProvider.GetRequiredService<IHomeService>();
        await rateService.UpdateCurrentStateAsync();
    }
}