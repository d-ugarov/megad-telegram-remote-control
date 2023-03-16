using System.Collections.Generic;

namespace SmartHome.Common.Models.Device;

public interface IDevice
{
    string Ip { get; }
    string Pwd { get; }
}

public interface IDevice<T> : IDevice where T : IDevicePort
{
    Dictionary<int, T> Ports { get; }
}