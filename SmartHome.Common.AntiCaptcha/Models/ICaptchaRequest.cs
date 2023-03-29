namespace SmartHome.Common.AntiCaptcha.Models;

internal interface ICaptchaRequest
{
    string ClientKey { get; init; }
}