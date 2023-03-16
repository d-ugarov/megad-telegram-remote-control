using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.Device;
using System;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Models.Home;

public class HomePortStatus
{
    public DevicePortStatus CurrentStatus { get; private set; }
    public DateTime CurrentSetDate { get; private set; }

    public DevicePortStatus? LastStatus { get; set; }
    public DateTime LastSetDate { get; set; }

    public HomePortStatus(DevicePortStatus status)
    {
        CurrentStatus = status;
        CurrentSetDate = DateTime.UtcNow;
    }

    public bool IsEqual(DevicePort port, DevicePortStatus newStatus)
    {
        return port is {Type: DevicePortType.IN} or {Type: DevicePortType.OUT, OutMode: DeviceOutPortMode.SW}
            ? CurrentStatus.InOutSwStatus == newStatus.InOutSwStatus
            : throw new Exception("Can't compare port status");
    }

    public void SetNewStatus(DevicePortStatus newStatus)
    {
        LastStatus = CurrentStatus;
        LastSetDate = CurrentSetDate;

        CurrentStatus = newStatus;
        CurrentSetDate = DateTime.UtcNow;
    }

    public bool TryGetDateAfterChange(out TimeSpan period)
    {
        if (LastStatus == null)
        {
            period = default;
            return false;
        }

        period = CurrentSetDate - LastSetDate;
        return true;
    }
}