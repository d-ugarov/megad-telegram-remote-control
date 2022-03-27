using MegaDTelegramRemoteControl.Infrastructure.Models;
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
    public static Task<OperationResult<HttpContent>> SendRequestSafeAsync(this HttpClient client, string url,
        HttpMethod method, object? query = null, object? body = null, TimeSpan timeout = default, 
        IEnumerable<(string key, string value)>? headers = null, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return InvokeOperations.InvokeOperationAsync(() =>
        {
            return client.SendRequestAsync(url, method, ObjectToParametersList(query), body, timeout, headers, jsonSerializerOptions);
        });
    }

    public static Task<OperationResult<T>> SendRequestSafeAsync<T>(this HttpClient client, string url,
        HttpMethod method, object? query = null, object? body = null, TimeSpan timeout = default,
        IEnumerable<(string key, string value)>? headers = null, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return InvokeOperations.InvokeOperationAsync(() =>
        {
            return client.SendRequestAsync<T>(url, method, query, body, timeout, headers, jsonSerializerOptions);
        });
    }

    public static async Task<T> SendRequestAsync<T>(this HttpClient client, string url, HttpMethod method,
        object? query = null, object? body = null, TimeSpan timeout = default,
        IEnumerable<(string key, string value)>? headers = null, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var content = await client.SendRequestAsync(url, method, ObjectToParametersList(query), body, timeout, headers, jsonSerializerOptions);
        return await content.ReadAsJsonAsync<T>();
    }

    public static Task<HttpContent> SendRequestAsync(this HttpClient client, string url, HttpMethod method,
        object? query = null, object? body = null, TimeSpan timeout = default,
        IEnumerable<(string key, string value)>? headers = null, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return client.SendRequestAsync(url, method, ObjectToParametersList(query), body, timeout, headers, jsonSerializerOptions);
    }

    public static async Task<T> SendRequestAsync<T>(this HttpClient client, string url, HttpMethod method,
        IEnumerable<(string key, object? value)> query, object? body = null, TimeSpan timeout = default,
        IEnumerable<(string key, string value)>? headers = null, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var content = await client.SendRequestAsync(url, method, query, body, timeout, headers, jsonSerializerOptions);
        return await content.ReadAsJsonAsync<T>();
    }

    public static async Task<HttpContent> SendRequestAsync(this HttpClient client, string url, HttpMethod method,
        IEnumerable<(string key, object? value)>? query, object? body = null, TimeSpan timeout = default,
        IEnumerable<(string key, string value)>? headers = null, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var response = await client.SendRequestWithRawResponseAsync(url, method, query, body, timeout, headers, jsonSerializerOptions);
        
        response.EnsureSuccessStatusCode();
        return response.Content;
    }

    public static async Task<HttpResponseMessage> SendRequestWithRawResponseAsync(this HttpClient client, string url,
        HttpMethod method, IEnumerable<(string key, object? value)>? query, object? body = null,
        TimeSpan timeout = default, IEnumerable<(string key, string value)>? headers = null,
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
        return await client.SendAsync(httpRequest, cts.Token).ConfigureAwait(false);
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

    private static IEnumerable<(string key, object? value)>? ObjectToParametersList(object? obj)
    {
        return obj?.GetType().GetProperties().SelectMany(x =>
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

    public static async ValueTask<T> ReadAsJsonAsync<T>(this HttpContent content, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        await using var stream = await content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, jsonSerializerOptions ?? PlatformJsonSerialization.Options) ??
               throw new Exception("Can't deserialize response");
    }
}