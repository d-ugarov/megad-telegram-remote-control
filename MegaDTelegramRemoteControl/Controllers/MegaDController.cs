using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Controllers
{
    public class MegaDController : Controller
    {
        private readonly IHomeLogic homeLogic;
        private readonly ILogger<MegaDController> logger;

        public MegaDController(IHomeLogic homeLogic, ILogger<MegaDController> logger)
        {
            this.homeLogic = homeLogic;
            this.logger = logger;
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