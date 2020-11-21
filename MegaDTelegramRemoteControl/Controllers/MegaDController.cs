using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Controllers
{
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
            var query = Request.Query.Select(x => (x.Key, x.Value.ToString())).ToList();

            var result = await homeLogic.OnNewEventAsync(id, query);
            
            if (result.IsSuccess && result.Data.SendCustomCommand)
                return Ok(result.Data.Command);
            
            return Ok("d");
        }
    }
}