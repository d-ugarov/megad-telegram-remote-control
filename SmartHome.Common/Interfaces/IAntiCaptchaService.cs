using SmartHome.Common.Infrastructure.Models;
using SmartHome.Common.Models.AntiCaptcha;
using System.Threading.Tasks;

namespace SmartHome.Common.Interfaces;

public interface IAntiCaptchaService
{
    Task<OperationResult<AntiCaptchaSolution>> SolveCaptchaAsync(AntiCaptchaRequest request);
}