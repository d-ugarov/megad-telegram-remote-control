using SmartHome.Common.AntiCaptcha.Configurations;
using SmartHome.Common.Helpers;
using SmartHome.Common.Interfaces;

namespace SmartHome.Common.AntiCaptcha;

public static class AntiCaptchaExtensions
{
    /// <summary>
    /// Inject <see cref="IAntiCaptchaService"/>
    /// </summary>
    public static void AddAntiCaptcha(this WebApplicationBuilder builder)
    {
        builder.ConfigureByType<AntiCaptchaConfig>();
        builder.Services.AddHttpClient<IAntiCaptchaService, AntiCaptchaService>();
    }
}