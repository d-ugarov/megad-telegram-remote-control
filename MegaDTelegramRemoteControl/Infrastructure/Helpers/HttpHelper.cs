using MegaDTelegramRemoteControl.Infrastructure.Models;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MegaDTelegramRemoteControl.Infrastructure.Helpers;

public static class HttpHelper
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public static Task<OperationResult<HttpContent>> SendRequestSafeAsync(this HttpClient client, string url,
        HttpMethod method, object? query = null, object? body = null, TimeSpan timeout = default, bool logErrors = true,
        IEnumerable<(string key, string value)>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null, bool throwErrorsFromContent = true)
    {
        return InvokeOperations.InvokeOperationAsync(() =>
        {
            return client.SendRequestAsync(url, method, ObjectToParametersList(query), body, timeout, logErrors,
                headers, jsonSerializerOptions, throwErrorsFromContent);
        });
    }

    public static Task<OperationResult<T>> SendRequestSafeAsync<T>(this HttpClient client, string url,
        HttpMethod method, object? query = null, object? body = null, TimeSpan timeout = default, bool logErrors = true,
        IEnumerable<(string key, string value)>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return InvokeOperations.InvokeOperationAsync(() =>
        {
            return client.SendRequestAsync<T>(url, method, query, body, timeout, logErrors, headers,
                jsonSerializerOptions);
        });
    }

    public static async Task<T> SendRequestAsync<T>(this HttpClient client, string url, HttpMethod method,
        object? query = null, object? body = null, TimeSpan timeout = default, bool logErrors = true,
        IEnumerable<(string key, string value)>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null, bool throwErrorsFromContent = true)
    {
        var content = await client.SendRequestAsync(url, method, ObjectToParametersList(query), body, timeout,
            logErrors, headers, jsonSerializerOptions, throwErrorsFromContent);
        return await content.ReadAsJsonAsync<T>();
    }

    public static Task<HttpContent> SendRequestAsync(this HttpClient client, string url, HttpMethod method,
        object? query = null, object? body = null, TimeSpan timeout = default, bool logErrors = true,
        IEnumerable<(string key, string value)>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null, bool throwErrorsFromContent = true)
    {
        return client.SendRequestAsync(url, method, ObjectToParametersList(query), body, timeout, logErrors,
            headers, jsonSerializerOptions, throwErrorsFromContent);
    }

    public static async Task<T> SendRequestAsync<T>(this HttpClient client, string url, HttpMethod method,
        IEnumerable<(string key, object? value)> query, object? body = null, TimeSpan timeout = default,
        IEnumerable<(string key, string value)>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null, bool throwErrorsFromContent = true)
    {
        var content = await client.SendRequestAsync(url, method, query, body, timeout, headers: headers,
            jsonSerializerOptions: jsonSerializerOptions, throwErrorsFromContent: throwErrorsFromContent);
        return await content.ReadAsJsonAsync<T>();
    }

    public static async Task<HttpContent> SendRequestAsync(this HttpClient client, string url, HttpMethod method,
        IEnumerable<(string key, object? value)>? query, object? body = null, TimeSpan timeout = default,
        bool logErrors = true, IEnumerable<(string key, string value)>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null, bool throwErrorsFromContent = true)
    {
        var response = await client.SendRequestWithRawResponseAsync(url, method, query, body, timeout, logErrors,
            headers, jsonSerializerOptions);
        try
        {
            if (throwErrorsFromContent)
            {
                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new Exception(!string.IsNullOrEmpty(content)
                        ? content
                        : $"Status code {response.StatusCode}");
                }
            }

            response.EnsureSuccessStatusCode();
            return response.Content;
        }
        catch (Exception e)
        {
            if (logErrors)
                logger.Trace($"HTTP request error: {method} {url} {response.StatusCode}\nException: {e}");
            throw;
        }
    }

    public static Task<HttpResponseMessage> SendRequestWithRawResponseAsync(this HttpClient client, string url,
        HttpMethod method, object? query = null, object? body = null, TimeSpan timeout = default,
        bool logErrors = true, IEnumerable<(string key, string value)>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return client.SendRequestWithRawResponseAsync(url, method, ObjectToParametersList(query), body, timeout,
            logErrors, headers, jsonSerializerOptions);
    }

    public static async Task<HttpResponseMessage> SendRequestWithRawResponseAsync(this HttpClient client, string url,
        HttpMethod method, IEnumerable<(string key, object? value)>? query, object? body = null,
        TimeSpan timeout = default, bool logErrors = true, IEnumerable<(string key, string value)>? headers = null,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        url = Regex.Replace(url, @"(?<!:)/+", @"/");

        if (client.BaseAddress != null && client.BaseAddress.ToString().EndsWith("/") && url.StartsWith("/"))
        {
            url = url.Substring(1, url.Length - 1);
        }

        if (query != null)
        {
            url = GenerateUrlWithParams(url, query);
        }

        using var httpRequest = new HttpRequestMessage(method, url);

        if (body != null)
        {
            if (body is HttpContent content)
            {
                httpRequest.Content = content;
            }
            else
            {
                var bodyStr = JsonSerializer.Serialize(body, jsonSerializerOptions ?? PlatformJsonSerialization.Options);
                httpRequest.Content = new StringContent(bodyStr, Encoding.UTF8, "application/json");
            }
        }

        if (headers != null)
        {
            foreach (var (key, value) in headers)
            {
                httpRequest.Headers.Add(key, new[] {value});
            }
        }

        if (timeout == default)
            timeout = TimeSpan.FromSeconds(100);

        var cts = new CancellationTokenSource(timeout);

        try
        {
            return await client.SendAsync(httpRequest, cts.Token).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            if (logErrors)
                logger.Trace($"HTTP request error: {method} {url}\nException: {e}");
            throw;
        }
    }

    public static string GenerateUrlWithParams(string url, IEnumerable<(string key, object? value)> parameters)
    {
        var queryStr = string.Join("&", parameters.Where(x => x.value != null)
                                                  .Select(x =>
                                                  {
                                                      var value = x.value is DateTime date
                                                          ? date.ToString("u")
                                                          : x.value!.ToString();

                                                      return $"{x.key}={HttpUtility.UrlEncode(value)}";
                                                  }));
        return $"{url}?{queryStr}";
    }

    private static IEnumerable<(string key, object? value)>? ObjectToParametersList(object? obj, bool arrayCommaSeparated = false)
    {
        var properties = obj?.GetType().GetProperties();

        return arrayCommaSeparated
            ? properties?.Select(x =>
            {
                var value = x.GetValue(obj, null);

                if (value is not string &&
                    value is IEnumerable array)
                {
                    return (x.Name, string.Join(",", array.Cast<object?>()));
                }

                return (x.Name, value);
            })
            : properties?.SelectMany(x =>
            {
                var value = x.GetValue(obj, null);

                if (value is not string &&
                    value is IEnumerable array)
                {
                    return array.Cast<object?>().Select(t => (x.Name, t));
                }

                return new[] {(x.Name, x.GetValue(obj, null))};
            });
    }

    public static async ValueTask<T> ReadAsJsonAsync<T>(this HttpContent content,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        await using var stream = await content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, jsonSerializerOptions ?? PlatformJsonSerialization.Options) ??
               throw new Exception("Can't deserialize response");
    }
}