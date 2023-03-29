namespace SmartHome.Common.AntiCaptcha.Models;

internal interface ICaptchaResponse
{
    int ErrorId { get; }
    string? ErrorCode { get; }
    string? ErrorDescription { get; }
}