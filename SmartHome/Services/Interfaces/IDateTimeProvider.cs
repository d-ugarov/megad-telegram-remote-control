using System;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}