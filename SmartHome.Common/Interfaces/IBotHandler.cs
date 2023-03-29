using SmartHome.Common.Models.Bot;
using System.Threading.Tasks;

namespace SmartHome.Common.Interfaces;

public interface IBotHandler
{
    Task<BotMenu> ProcessActionAsync(string? actionId = null);
}