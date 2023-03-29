using Microsoft.Extensions.Options;
using SmartHome.Common.AntiCaptcha.Configurations;
using SmartHome.Common.AntiCaptcha.Models;
using SmartHome.Common.Infrastructure.Helpers;
using SmartHome.Common.Infrastructure.Models;
using SmartHome.Common.Interfaces;
using SmartHome.Common.Models.AntiCaptcha;

namespace SmartHome.Common.AntiCaptcha;

internal class AntiCaptchaService : IAntiCaptchaService
{
    private readonly HttpClient httpClient;
    private readonly AntiCaptchaConfig config;

    private const string antiCaptchaUrl = "https://api.anti-captcha.com/";
    private const string antiCaptchaRouteCreateTask = "createTask";
    private const string antiCaptchaRouteGetTaskResult = "getTaskResult";

    private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(30);

    public AntiCaptchaService(HttpClient httpClient, IOptions<AntiCaptchaConfig> config)
    {
        this.httpClient = httpClient;
        this.config = config.Value;
    }

    public Task<OperationResult<AntiCaptchaSolution>> SolveCaptchaAsync(AntiCaptchaRequest request)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            if (string.IsNullOrEmpty(config.ApiKey))
                throw new Exception("Empty AntiCaptcha API-Key");

            switch (request.Type)
            {
                case AntiCaptchaType.ReCaptchaV2 when request.ReCaptchaV2 != null:
                {
                    var result = await SolveReCaptchaV2Async(request.ReCaptchaV2);
                    return new AntiCaptchaSolution(AntiCaptchaType.ReCaptchaV2, ReCaptchaV2: new(result.GRecaptchaResponse));
                }
                default:
                    throw new NotSupportedException($"Captcha type {request.Type} not supported");
            }
        });
    }

    private async Task<CaptchaSolution> SolveReCaptchaV2Async(AntiCaptchaReCaptchaV2 request)
    {
        var createTaskRequest = new CaptchaCreateTaskRequest
                                {
                                    ClientKey = config.ApiKey,
                                    Task = new CaptchaTask
                                           {
                                               Type = "RecaptchaV2TaskProxyless",
                                               IsInvisible = request.IsInvisible,
                                               WebsiteKey = request.WebsiteKey,
                                               WebsiteUrl = request.WebsiteUrl,
                                           },
                                };
        var createTaskResult = await httpClient.SendRequestAsync<CaptchaCreateTaskResponse>(
            antiCaptchaUrl + antiCaptchaRouteCreateTask,
            HttpMethod.Post, body: createTaskRequest, timeout: defaultTimeout);
        CheckResponse(createTaskResult);

        var deadline = DateTime.UtcNow.AddSeconds(config.MaxSolveDurationInSeconds);
        await Task.Delay(TimeSpan.FromSeconds(5));

        while (true)
        {
            var getTaskRequest = new CaptchaGetTaskRequest
                                 {
                                     ClientKey = config.ApiKey,
                                     TaskId = createTaskResult.TaskId,
                                 };
            var getTaskResult = await httpClient.SendRequestAsync<CaptchaGetTaskResponse>(
                antiCaptchaUrl + antiCaptchaRouteGetTaskResult,
                HttpMethod.Post, body: getTaskRequest, timeout: defaultTimeout);
            CheckResponse(getTaskResult);

            if (getTaskResult.Status == CaptchaGetTaskResponseStatus.Ready)
                return getTaskResult.Solution ?? throw new Exception("Solution is empty");

            if (DateTime.UtcNow > deadline)
                break;

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        throw new Exception("Solving captcha time is over");
    }

    private static void CheckResponse(ICaptchaResponse response)
    {
        if (response.ErrorId == 0)
            return;

        throw new Exception($"{response.ErrorDescription} ({response.ErrorCode})");
    }
}