﻿namespace MegaDTelegramRemoteControl.Infrastructure.Configurations;

public record PlatformConfig
{
    public bool UseFakeDeviceConnector { get; init; }
}