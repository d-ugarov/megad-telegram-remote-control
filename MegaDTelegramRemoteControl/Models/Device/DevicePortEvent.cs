using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using System;
using System.Text;

namespace MegaDTelegramRemoteControl.Models.Device;

public record DevicePortEvent
{
    private DevicePortInEvent? inPortData;
    private DevicePortOutEvent? outPortData;

    public required DevicePort Port { get; init; }

    public required DateTime Date { get; init; }

    public DevicePortInEvent? In
    {
        get => Port.Type == DevicePortType.IN
            ? inPortData
            : throw new OperationException($"'IN' port data is not available for port type {Port.Type}");
        set => inPortData = value;
    }

    public DevicePortOutEvent? Out
    {
        get => Port.Type == DevicePortType.OUT
            ? outPortData
            : throw new OperationException($"'Out' port data is not available for port type {Port.Type}");
        set => outPortData = value;
    }

    public override string ToString()
    {
        var str = new StringBuilder();

        str.Append($"port: {Port.Name} ({Port.Type})");

        if (inPortData != null)
            str.Append($", {inPortData}");

        if (outPortData != null)
            str.Append($", {outPortData}");

        return str.ToString();
    }
}