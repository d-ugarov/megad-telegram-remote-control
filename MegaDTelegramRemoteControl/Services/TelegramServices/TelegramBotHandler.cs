using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Location = MegaDTelegramRemoteControl.Models.Device.Location;

namespace MegaDTelegramRemoteControl.Services.TelegramServices
{
    public class TelegramBotHandler : IBotHandler
    {
        private readonly HomeConfig homeConfig;
        private readonly IDeviceConnector deviceConnector;
        private readonly ILogger<TelegramBotHandler> logger;

        public TelegramBotHandler(HomeConfig homeConfig, 
            IDeviceConnector deviceConnector, 
            ILogger<TelegramBotHandler> logger)
        {
            this.homeConfig = homeConfig;
            this.deviceConnector = deviceConnector;
            this.logger = logger;
        }

        public async Task<BotMenu> ProcessActionAsync(string? actionId = null)
        {
            if (string.IsNullOrEmpty(actionId))
                return GetDefaultMenu();

            return await ProcessActionInternalAsync(homeConfig.Locations, actionId) ?? GetDefaultMenu();
        }

        private BotMenu GetDefaultMenu()
        {
            logger.LogTrace($"[BotHandler] Return default menu");
            
            var result = new BotMenu
                         {
                             Text = "Dashboard",
                             Buttons = homeConfig.Locations
                                                 .Select(x => new ButtonItem
                                                              {
                                                                  Id = x.Id,
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
                    if (item.Id == id)
                    {
                        logger.LogTrace($"[BotHandler] Call: {location.Name} -> {item.Port.Name}");

                        await deviceConnector.InvokePortActionAsync(item.Port, DevicePortAction.DefaultSwitch);
                        
                        return await CreateLocationPageAsync(location);
                    }
                }

                var subLocationPage = await ProcessActionInternalAsync(location.SubLocations, id);
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
                                       Id = subLocation.Id,
                                       Name = subLocation.Name,
                                   });
            }

            foreach (var items in location.Items.Where(x => x.Port.Type == DevicePortType.OUT))
            {
                var item = new ButtonItem
                           {
                               Id = items.Id,
                               Name = items.CustomName ?? items.Port.Name,
                           };

                var status = await deviceConnector.GetPortStatusAsync(items.Port);
                if (status.IsSuccess)
                    item.Name = $"{status.Data} {item.Name}";

                result.Buttons.Add(item);
            }

            AppendNavigationButtons(location, result);
            
            return result;
        }

        private static void AppendNavigationButtons(Location location, BotMenu menu)
        {
            if (location.Parent != null)
            {
                menu.Buttons.Add(new ButtonItem
                                 {
                                     Id = location.Parent.Id,
                                     Name = "« Back",
                                     Order = int.MaxValue,
                                 });
            }
            
            menu.Buttons.Add(new ButtonItem
                             {
                                 Id = Guid.NewGuid().ToString(),
                                 Name = "Dashboard",
                                 Order = int.MaxValue,
                             });
        }
    }
}