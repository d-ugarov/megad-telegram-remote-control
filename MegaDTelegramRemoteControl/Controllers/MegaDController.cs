using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Controllers;

public class MegaDController : Controller
{
    private readonly IHomeLogic homeLogic;

    public MegaDController(IHomeLogic homeLogic)
    {
        this.homeLogic = homeLogic;
    }

    [HttpGet]
    [Route("megad/{id}")]
    public async Task<IActionResult> OnNewEvent(string id)
    {
        var eventData = Request.Query.Select(x => new NewEventData(x.Key, x.Value.ToString())).ToList();

        var result = await homeLogic.OnNewEventAsync(id, eventData);

        if (result.IsSuccess && result.Data!.SendOk200)
        {
            return result.Data!.SendOk200Data switch
            {
                not null => Ok(result.Data!.SendOk200Data),
                _ => Ok(),
            };
        }

        return NoContent();
    }
}