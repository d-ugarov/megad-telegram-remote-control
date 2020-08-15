using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Location = MegaDTelegramRemoteControl.Models.Device.Location;

namespace MegaDTelegramRemoteControl.Services
{
    public class TelegramLogic : ITelegramLogic
    {
        private readonly HomeConfig homeConfig;
        private readonly IDeviceConnector deviceConnector;

        public TelegramLogic(HomeConfig homeConfig, IDeviceConnector deviceConnector)
        {
            this.homeConfig = homeConfig;
            this.deviceConnector = deviceConnector;
        }

        public Task<OperationResult<TelegramBotMenu>> GetTelegramBotMenuAsync(string currentId = null)
        {
            return InvokeOperations.InvokeOperationAsync(async () =>
            {
                if (string.IsNullOrEmpty(currentId))
                    return GetDefaultMenu();
                
                return await GetMenuByIdAsync(homeConfig.Locations, currentId) ?? GetDefaultMenu();
            });
        }

        private TelegramBotMenu GetDefaultMenu()
        {
            var result = new TelegramBotMenu
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

        private async Task<TelegramBotMenu> GetMenuByIdAsync(IEnumerable<Location> locations, string id)
        {
            foreach (var location in locations)
            {
                if (location.Id == id)
                {
                    var result = new TelegramBotMenu
                                 {
                                     Text = location.Name,
                                     Buttons = new List<ButtonItem>(),
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
                                       Name = items.Port.Name,
                                   };
                        
                        var status = await deviceConnector.GetPortStatusAsync(items.Port);
                        if (status.IsSuccess)
                            item.Name += $": {status.Data}";
                        
                        result.Buttons.Add(item);
                    }
                    
                    AppendNavigationButtons(location, result);

                    return result;
                }

                var subLocationResult = await GetMenuByIdAsync(location.SubLocations, id);
                if (subLocationResult != null)
                    return subLocationResult;
            }

            return null;
        }

        private static void AppendNavigationButtons(Location location, TelegramBotMenu menu)
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