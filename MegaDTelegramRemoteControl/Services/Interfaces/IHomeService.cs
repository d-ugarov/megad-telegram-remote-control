using MegaDTelegramRemoteControl.Models.Device;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IHomeService
{
    Dictionary<string, Device> Devices { get; }
    List<Location> Locations { get; }

    Task UpdateCurrentStateAsync();
}