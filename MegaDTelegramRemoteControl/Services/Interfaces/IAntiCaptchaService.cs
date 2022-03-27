using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.AntiCaptcha;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IAntiCaptchaService
{
    Task<OperationResult<CaptchaSolution>> SolveReCaptchaV2Async(ReCaptchaV2Task task);
}