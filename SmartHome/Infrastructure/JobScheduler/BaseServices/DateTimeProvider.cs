using MegaDTelegramRemoteControl.Services.Interfaces;
using System;

namespace MegaDTelegramRemoteControl.Infrastructure.JobScheduler.BaseServices;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}