using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Models.Device.Enums;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Device = MegaDTelegramRemoteControl.Models.Device.Device;
using DevicePort = MegaDTelegramRemoteControl.Models.Device.DevicePort;

namespace MegaDTelegramRemoteControl.Services;

public class HomeService : IHomeService
{
    private readonly Dictionary<Device, Dictionary<DevicePort, List<DevicePortEvent>>> current;
    private readonly ReaderWriterLockSlim locker;
    private readonly ILogger<HomeService> logger;
    private readonly HomeConfig homeConfig;

    public HomeService(ILogger<HomeService> logger, HomeConfig homeConfig)
    {
        current = new Dictionary<Device, Dictionary<DevicePort, List<DevicePortEvent>>>();
        locker = new ReaderWriterLockSlim();

        this.logger = logger;
        this.homeConfig = homeConfig;
    }

    public Dictionary<string, Device> Devices => homeConfig.Devices;

    public List<Location> Locations => homeConfig.Locations;

    #region Set

    public void Set(DeviceEvent deviceEvent)
    {
        try
        {
            if (!deviceEvent.IsParsedSuccessfully)
                return;

            locker.EnterWriteLock();

            switch (deviceEvent.Type)
            {
                case DeviceEventType.DeviceStarted:
                    SetDeviceStarted(deviceEvent);
                    break;
                case DeviceEventType.PortEvent:
                    SetPortEvent(deviceEvent);
                    break;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            throw;
        }
        finally
        {
            locker.ExitWriteLock();
        }
    }

    private void SetDeviceStarted(DeviceEvent deviceEvent)
    {
        current.Remove(deviceEvent.Device!);
    }

    private void SetPortEvent(DeviceEvent deviceEvent)
    {
        if (!current.TryGetValue(deviceEvent.Device!, out var statePorts))
        {
            statePorts = new Dictionary<DevicePort, List<DevicePortEvent>>();
            current.Add(deviceEvent.Device!, statePorts);
        }

        if (!statePorts.TryGetValue(deviceEvent.Event!.Port, out var events))
        {
            events = new List<DevicePortEvent>();
            statePorts[deviceEvent.Event.Port] = events;
        }

        events.Add(deviceEvent.Event);
    }

    #endregion

    #region Get

    public DevicePortEvent? Get(DevicePort port)
    {
        try
        {
            locker.EnterReadLock();

            return default;
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            throw;
        }
        finally
        {
            locker.ExitReadLock();
        }
    }

    #endregion

    #region Clean

    public void Clean()
    {
        try
        {
            locker.EnterWriteLock();

            foreach (var ports in current.Values)
            {
                foreach (var port in ports.Keys)
                {
                    var portEvents = ports[port];

                    if (!portEvents.Any())
                        continue;

                    ports[port] = portEvents.Where(x => x.Date > DateTime.UtcNow.AddHours(-1)).ToList();
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
            throw;
        }
        finally
        {
            locker.ExitWriteLock();
        }
    }

    #endregion
}