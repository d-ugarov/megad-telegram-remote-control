using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Helpers;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.AntiCaptcha;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.PrivateOffices.AntiCaptcha;

public class AntiCaptchaService : IAntiCaptchaService
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

    public Task<OperationResult<CaptchaSolution>> SolveReCaptchaV2Async(ReCaptchaV2Task task)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            if (string.IsNullOrEmpty(config.ApiKey))
                throw new Exception("Empty AntiCaptcha API-Key");

            var createTaskRequest = new CaptchaCreateTaskRequest
                                    {
                                        ClientKey = config.ApiKey,
                                        Task = new()
                                               {
                                                   Type = "RecaptchaV2TaskProxyless",
                                                   IsInvisible = task.IsInvisible,
                                                   WebsiteKey = task.WebsiteKey,
                                                   WebsiteUrl = task.WebsiteUrl,
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
        });
    }

    private static void CheckResponse(ICaptchaResponse response)
    {
        if (response.ErrorId == 0)
            return;

        throw new Exception($"{response.ErrorDescription} ({response.ErrorCode})");
    }
}