using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Infrastructure.Jobs.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Jobs.Enums;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Infrastructure.Jobs.Services
{
    public class JobScheduler : IHostedService, IDisposable
    {
        private IServiceProvider Services { get; }
        private readonly InternalSchedulerConfig configuration;
        private readonly ILogger<JobScheduler> logger;

        private readonly Dictionary<JobType, Job> activeJobs;
        private volatile bool canWork = false;
        
        public JobScheduler(IServiceProvider services, InternalSchedulerConfig configuration, ILogger<JobScheduler> logger)
        {
            this.Services = services;
            this.configuration = configuration;
            this.logger = logger;

            activeJobs = new Dictionary<JobType, Job>();
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(InvokeOperations.InvokeOperation(() =>
            {
                using var scope = Services.CreateScope();
                canWork = true;

                if (configuration?.Jobs != null)
                {
                    foreach (var jobType in configuration.Jobs)
                    {
                        var jobTypeInfo = jobType.GetAttribute<JobTypeAttribute>();

                        logger.LogInformation($"[JobScheduler] Background service '{jobType}' started");

                        activeJobs[jobType] = new Job
                                              {
                                                  Timer = new Timer(async state => await ProcessJobAsync(state),
                                                      jobType, jobTypeInfo.DueTime, jobTypeInfo.Period),
                                                  Interval = jobTypeInfo.Interval
                                              };
                    }
                }

                if (!configuration?.Jobs?.Any() ?? true)
                    logger.LogInformation("[JobScheduler] Background services are disabled");
            }));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            canWork = false;

            if (activeJobs.Any())
            {
                logger.LogInformation("[JobScheduler] Background services are stopping");

                foreach (var job in activeJobs)
                {
                    job.Value.Timer?.Change(Timeout.Infinite, 0);
                }
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            canWork = false;

            foreach (var job in activeJobs)
            {
                job.Value.Timer?.Dispose();
            }
        }

        private async Task ProcessJobAsync(object state)
        {
            Job job = null;

            await InvokeOperations.InvokeOperationAsync(async () =>
            {
                var type = (JobType)state;
                if (!activeJobs.TryGetValue(type, out job))
                    throw new Exception($"Can't find job with type {type}");

                job.Timer?.Change(Timeout.Infinite, Timeout.Infinite);

                using var scope = Services.CreateScope();
                
                switch (type)
                {
                    case JobType.Test:
                        await ProcessOtherJobsAsync(scope);
                        break;
                }
            });

            if (canWork)
                job?.Timer?.Change(job.Interval, TimeSpan.FromDays(1));
        }

        #region Jobs

        public Task ProcessOtherJobsAsync(IServiceScope scope)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}