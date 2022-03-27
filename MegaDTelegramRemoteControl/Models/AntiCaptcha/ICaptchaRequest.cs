namespace MegaDTelegramRemoteControl.Models.AntiCaptcha;

public interface ICaptchaRequest
{
    string ClientKey { get; init; }
}