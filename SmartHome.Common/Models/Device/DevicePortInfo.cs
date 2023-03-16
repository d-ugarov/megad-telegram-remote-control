using SmartHome.Common.Infrastructure.Models;
using SmartHome.Common.Models.Device.Enums;

namespace SmartHome.Common.Models.Device;

public record DevicePortInfo(IDevicePort Port)
{
    private DevicePortStatusIn? statusIn;
    private DevicePortStatusOut? statusOut;

    public DevicePortStatusIn? StatusIn
    {
        get => Port.Type == DevicePortType.IN
            ? statusIn
            : throw new OperationException($"'IN' port status is not available for port type {Port.Type}");
        set => statusIn = value;
    }

    public DevicePortStatusOut? StatusOut
    {
        get => Port.Type == DevicePortType.OUT
            ? statusOut
            : throw new OperationException($"'OUT' port status is not available for port type {Port.Type}");
        set => statusOut = value;
    }
}

public record struct DevicePortStatusIn(DevicePortInOutSWStatus InOutSwStatus, int InCounter);

public record struct DevicePortStatusOut(DevicePortInOutSWStatus InOutSwStatus);