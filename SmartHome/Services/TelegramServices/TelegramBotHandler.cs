using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Location = MegaDTelegramRemoteControl.Models.Device.Location;

namespace MegaDTelegramRemoteControl.Services.TelegramServices;

public class TelegramBotHandler : IBotHandler
{
    private readonly IHomeService homeService;
    private readonly IDeviceConnector deviceConnector;
    private readonly ILogger<TelegramBotHandler> logger;

    public TelegramBotHandler(IHomeService homeService,
        IDeviceConnector deviceConnector,
        ILogger<TelegramBotHandler> logger)
    {
        this.homeService = homeService;
        this.deviceConnector = deviceConnector;
        this.logger = logger;
    }

    public async Task<BotMenu> ProcessActionAsync(string? actionId = null)
    {
        if (string.IsNullOrEmpty(actionId))
            return GetDefaultMenu();

        return await ProcessActionInternalAsync(homeService.Locations, actionId) ??
               await ProcessActionConditionInternalAsync(homeService.Locations, actionId) ??
               GetDefaultMenu();
    }

    private BotMenu GetDefaultMenu()
    {
        logger.LogTrace("[BotHandler] Return default menu");

        var result = new BotMenu
                     {
                         Text = "Dashboard",
                         Buttons = homeService.Locations
                                             .Select(x => new ButtonItem
                                                          {
                                                              ActionId = x.Id,
                                                              Name = x.Name,
                                                          })
                                             .ToList(),
                     };
        return result;
    }

    private async Task<BotMenu?> ProcessActionInternalAsync(IEnumerable<Location> locations, string id)
    {
        foreach (var location in locations)
        {
            // page by location id
            if (location.Id == id)
            {
                return await CreateLocationPageAsync(location);
            }

            foreach (var item in location.Items)
            {
                // action by item id
                if (item.ActionId == id)
                {
                    logger.LogTrace($"[BotHandler] Call: {location.Name} -> {item.Port.Name}");

                    await deviceConnector.InvokePortActionAsync(item.Port, DevicePortAction.CommandSWDefault);

                    return await CreateLocationPageAsync(location);
                }
            }

            var subLocationPage = await ProcessActionInternalAsync(location.SubLocations, id);
            if (subLocationPage != null)
                return subLocationPage;
        }

        return null;
    }

    private async Task<BotMenu?> ProcessActionConditionInternalAsync(IEnumerable<Location> locations, string id)
    {
        foreach (var location in locations)
        {
            foreach (var condition in location.ItemsConditions)
            {
                foreach (var item in condition.Items)
                {
                    // action by item id
                    if (item.ActionId == id)
                    {
                        logger.LogTrace($"[BotHandler] Call: {location.Name} -> {item.Port.Name}");

                        await deviceConnector.InvokePortActionAsync(item.Port, DevicePortAction.CommandSWDefault);

                        return await CreateLocationPageAsync(location);
                    }
                }
            }

            var subLocationPage = await ProcessActionConditionInternalAsync(location.SubLocations, id);
            if (subLocationPage != null)
                return subLocationPage;
        }

        return null;
    }

    private async Task<BotMenu> CreateLocationPageAsync(Location location)
    {
        logger.LogTrace($"[BotHandler] Return location: {location.Name}");

        var result = new BotMenu
                     {
                         Text = location.Name,
                     };

        foreach (var subLocation in location.SubLocations)
        {
            result.Buttons.Add(new ButtonItem
                               {
                                   ActionId = subLocation.Id,
                                   Name = $"{subLocation.Name} ▶",
                               });
        }

        var devicesStatuses = new Dictionary<string, List<DevicePortInfo>>();

        foreach (var item in location.Items)
        {
            var status = await GetPortStatusAsync(devicesStatuses, item.Port);

            result.Buttons.Add(new ButtonItem
                               {
                                   ActionId = item.ActionId,
                                   Name = status != null
                                       ? $"{status} {item.FormattedName}"
                                       : $"{item.FormattedName}",
                               });
        }

        foreach (var condition in location.ItemsConditions)
        {
            foreach (var item in condition.Items)
            {
                var status = await GetPortStatusAsync(devicesStatuses, item.Port);

                switch (condition.Type)
                {
                    case LocationConditionType.PortOutCurrentStatus when status?.Status.InOutSwStatus == condition.Status:
                    {
                        result.Buttons.Add(new ButtonItem
                                           {
                                               ActionId = item.ActionId,
                                               Name = $"{status} {item.FormattedName}",
                                           });
                        break;
                    }
                }
            }
        }

        AppendNavigationButtons(location, result);

        return result;
    }

    private async Task<DevicePortInfo?> GetPortStatusAsync(IDictionary<string, List<DevicePortInfo>> devicesStatuses, DevicePort port)
    {
        if (!devicesStatuses.TryGetValue(port.Device.Id, out var statuses))
        {
            var deviceStatuses = await deviceConnector.GetDevicePortsStatusesAsync(port.Device);
            
            statuses = deviceStatuses.IsSuccess
                ? deviceStatuses.Data!
                : new();
            
            devicesStatuses.Add(port.Device.Id, statuses);
        }

        return statuses.FirstOrDefault(x => x.Port.Id == port.Id);
    }

    private static void AppendNavigationButtons(Location location, BotMenu menu)
    {
        if (location.Parent != null)
        {
            menu.FooterButtons.Add(new ButtonItem
                                   {
                                       ActionId = location.Parent.Id,
                                       Name = "↩ Back",
                                       Order = int.MaxValue,
                                   });
        }

        menu.FooterButtons.Add(new ButtonItem
                               {
                                   ActionId = Guid.NewGuid().ToString(),
                                   Name = "🔼 Dashboard",
                                   Order = int.MaxValue,
                               });
    }
}