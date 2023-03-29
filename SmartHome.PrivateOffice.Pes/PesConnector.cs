using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SmartHome.Common.Infrastructure.Helpers;
using SmartHome.Common.Infrastructure.Models;
using SmartHome.Common.Interfaces;
using SmartHome.Common.Models.AntiCaptcha;
using SmartHome.PrivateOffice.Pes.Configurations;
using SmartHome.PrivateOffice.Pes.Models;
using System.Net;

namespace SmartHome.PrivateOffice.Pes;

public class PesConnector
{
    private readonly HttpClient httpClient;
    private readonly IMemoryCache memoryCache;
    private readonly IAntiCaptchaService antiCaptchaService;
    private readonly PesCommonConfig pesCommonConfig;
    
    private const string pesBaseUrl = "https://ikus.pesc.ru/";
    private const string pesApiUrl = "https://ikus.pesc.ru/application/";
    private const string pesApiRouteLogin = "v3/auth/login";
    private const string pesApiRouteGroups = "v3/groups";
    private const string pesApiRouteAccounts = "v5/groups/{0}/accounts";
    private const string pesApiRouteAccountData = "v5/accounts/{0}/data";
    
    private static readonly TimeSpan defaultTimeout = TimeSpan.FromSeconds(30);

    public PesConnector(HttpClient httpClient,
        IMemoryCache memoryCache,
        IAntiCaptchaService antiCaptchaService,
        IOptions<PesConfig> pesConfig)
    {
        this.httpClient = httpClient;
        this.memoryCache = memoryCache;
        this.antiCaptchaService = antiCaptchaService;
        this.pesCommonConfig = pesConfig.Value.CommonConfig;
    }

    internal Task<OperationResult<List<PesGroup>>> GetGroupsAsync(PesClientConfig config)
    {
        return InvokeOperations.InvokeOperationAsync(() =>
        {
            const string url = pesApiUrl + pesApiRouteGroups;
            return ExecuteRequestAsync<List<PesGroup>>(config, url, HttpMethod.Get);
        });
    }

    internal Task<OperationResult<List<PesAccount>>> GetAccountsAsync(PesClientConfig config, int groupId)
    {
        return InvokeOperations.InvokeOperationAsync(() =>
        {
            var url = pesApiUrl + string.Format(pesApiRouteAccounts, groupId);
            return ExecuteRequestAsync<List<PesAccount>>(config, url, HttpMethod.Get);
        });
    }

    internal Task<OperationResult<PesData>> GetAccountDataAsync(PesClientConfig config, int accountId)
    {
        return InvokeOperations.InvokeOperationAsync(() =>
        {
            var url = pesApiUrl + string.Format(pesApiRouteAccountData, accountId);
            return ExecuteRequestAsync<PesData>(config, url, HttpMethod.Get);
        });
    }
    
    #region Private methods

    private async Task<T> ExecuteRequestAsync<T>(PesClientConfig config, string url, HttpMethod method, 
        object? query = null, object? body = null, TimeSpan? timeout = null)
    {
        async Task<HttpResponseMessage> ExecuteAsync(bool forceGetAuthToken)
        {
            var token = await GetAuthTokenAsync(config, forceGetAuthToken);

            return await httpClient.SendRequestWithRawResponseAsync(url, method,
                query: query,
                body: body,
                timeout: timeout ?? defaultTimeout,
                headers: new List<(string key, string value)> {("Authorization", $"Bearer {token}")});
        }

        var result = await ExecuteAsync(false);

        if (result.StatusCode == HttpStatusCode.Unauthorized)
            result = await ExecuteAsync(true);
        
        if (result.IsSuccessStatusCode)
            return await result.Content.ReadAsJsonAsync<T>();

        var errorMsg = await result.Content.ReadAsStringAsync();

        throw new Exception($"Can't execute request {url}: {result.StatusCode} {errorMsg}");
    }

    private async Task<string> GetAuthTokenAsync(PesClientConfig config, bool force = false)
    {
        var key = $"PesAuthTokensCache:{config.Username}";

        if (!force && memoryCache.TryGetValue(key, out string? token) && !string.IsNullOrEmpty(token))
            return token;

        var (jwt, lifetime) = await AuthorizeAsync(config);

        memoryCache.Set(key, jwt, lifetime);

        return jwt;
    }

    private async Task<(string Token, TimeSpan Lifetime)> AuthorizeAsync(PesClientConfig config)
    {
        if (string.IsNullOrEmpty(config.Password) || string.IsNullOrEmpty(config.Username))
            throw new Exception("Empty config");

        var captchaCode = (string?)null;
        
        if (!string.IsNullOrEmpty(pesCommonConfig.CaptchaWebsiteKey))
        {
            var captchaResult = await antiCaptchaService.SolveCaptchaAsync(
                new(AntiCaptchaType.ReCaptchaV2,
                    ReCaptchaV2: new(pesBaseUrl, pesCommonConfig.CaptchaWebsiteKey, true)));
            if (captchaResult.IsSuccess && captchaResult.Data!.ReCaptchaV2 != null)
                captchaCode = captchaResult.Data!.ReCaptchaV2.GRecaptchaResponse;
        }

        var body = new
                   {
                       Username = config.Username,
                       Password = config.Password,
                       CaptchaCode = captchaCode,
                   };

        var result = await httpClient.SendRequestAsync<PesAuthResult>(pesApiUrl + pesApiRouteLogin, HttpMethod.Post,
            body: body, timeout: defaultTimeout);

        if (string.IsNullOrEmpty(result.Token))
            throw new Exception("Can't authorize in PES");

        var token = new JsonWebTokenHandler().ReadToken(result.Token);
        var lifetime = token.ValidTo > DateTime.UtcNow
            ? token.ValidTo - DateTime.UtcNow - TimeSpan.FromMinutes(5)
            : TimeSpan.FromDays(1);

        return (result.Token, lifetime);
    }
    
    #endregion
}