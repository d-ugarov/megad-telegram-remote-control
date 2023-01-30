using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaDTelegramRemoteControl.Infrastructure.Models;

public interface IOperationResult
{
    bool IsSuccess { get; }
    ErrorCodes? ErrorCode { get; }
    object? ErrorData { get; }
    bool HasErrorMessages { get; }
    List<string>? Errors { get; }
    Exception? Exception { get; }

    void EnsureSuccess();
}
    
public interface IDataOperationResult<out T> : IOperationResult
{
    T? Data { get; }
}

public record OperationResult : IOperationResult
{
    public bool IsSuccess { get; private init; }

    public ErrorCodes? ErrorCode { get; private init; }
    public object? ErrorData { get; private init; }
    public bool HasErrorMessages => Errors?.Any() ?? false;
    public List<string>? Errors { get; private init; }
    public Exception? Exception { get; private init; }

    public void EnsureSuccess()
    {
        if (IsSuccess)
            return;

        if (Exception != null)
            throw Exception;

        if (HasErrorMessages)
            throw new Exception(this.Report());

        throw new Exception("Unknown error");
    }

    public static OperationResult Ok() => new()
                                          {
                                              IsSuccess = true,
                                          };

    public static OperationResult Failed(Exception? exception = null,
        ErrorCodes errorCode = default, object? errorData = null)
    {
        return new()
               {
                   IsSuccess = false,
                   Errors = new List<string>(),
                   Exception = exception,
                   ErrorCode = errorCode,
                   ErrorData = errorData,
               };
    }

    public static OperationResult Failed(string error, Exception? exception = null,
        ErrorCodes errorCode = default, object? errorData = null)
    {
        return new()
               {
                   IsSuccess = false,
                   Errors = new List<string> {error},
                   Exception = exception,
                   ErrorCode = errorCode,
                   ErrorData = errorData,
               };
    }

    public static OperationResult Failed(IEnumerable<string>? errors, Exception? exception = null,
        ErrorCodes errorCode = default, object? errorData = null)
    {
        return new()
               {
                   IsSuccess = false,
                   Errors = errors?.ToList(),
                   Exception = exception,
                   ErrorCode = errorCode,
                   ErrorData = errorData,
               };
    }
}

public record OperationResult<T> : IDataOperationResult<T>
{
    public bool IsSuccess { get; private init; }
        
    public ErrorCodes? ErrorCode { get; private init; }
    public object? ErrorData { get; private init; }
    public bool HasErrorMessages => Errors?.Any() ?? false;
    public List<string>? Errors { get; private init; }
    public Exception? Exception { get; private init; }

    private readonly T? data;
        
    public T? Data
    {
        get => data ?? default;
        private init => data = value;
    }

    public T DataUnsafe
    {
        get
        {
            EnsureSuccess();
            return Data!;
        }
    }

    public void EnsureSuccess()
    {
        if (IsSuccess)
            return;
            
        if (Exception != null)
            throw Exception;

        if (HasErrorMessages)
            throw new Exception(this.Report());
            
        throw new Exception("Unknown error");
    }

    public OperationResult ActionResult => IsSuccess
        ? OperationResult.Ok()
        : OperationResult.Failed(Errors, Exception, ErrorCode ?? default, ErrorData);

    public static OperationResult<T> Ok() => new()
                                             {
                                                 IsSuccess = true,
                                             };

    public static OperationResult<T> Ok(T result) => new()
                                                     {
                                                         IsSuccess = true,
                                                         Data = result,
                                                     };

    public static OperationResult<T> Failed(Exception? exception = null, 
        ErrorCodes errorCode = default, object? errorData = null)
    {
        return new()
               {
                   IsSuccess = false,
                   Errors = new List<string>(),
                   Exception = exception,
                   ErrorCode = errorCode,
                   ErrorData = errorData,
               };
    }

    public static OperationResult<T> Failed(string error, Exception? exception = null, 
        ErrorCodes errorCode = default, object? errorData = null)
    {
        return new()
               {
                   IsSuccess = false,
                   Errors = new List<string> {error},
                   Exception = exception,
                   ErrorCode = errorCode,
                   ErrorData = errorData,
               };
    }

    public static OperationResult<T> Failed(IEnumerable<string> errors, Exception? exception = null, 
        ErrorCodes errorCode = default, object? errorData = null)
    {
        return new()
               {
                   IsSuccess = false,
                   Errors = errors.ToList(),
                   Exception = exception,
                   ErrorCode = errorCode,
                   ErrorData = errorData,
               };
    }
}

public static class OperationResultHelper
{
    public static string Report(this IOperationResult operationResult, ReportLevel reportLevel = ReportLevel.Default)
    {
        if (operationResult.IsSuccess)
            return "OK";

        const string failed = "Failed";

        switch (reportLevel)
        {
            case ReportLevel.WithExceptionMessage:
            {
                return operationResult.Errors?.Any() ?? false
                    ? $"{failed} ({string.Join(", ", operationResult.Errors)})"
                    : !string.IsNullOrEmpty(operationResult.Exception?.Message)
                        ? $"{failed} ({operationResult.Exception.Message})"
                        : failed;
            }
            case ReportLevel.WithExceptionFull:
            {
                var msg = operationResult.Errors?.Any() ?? false
                    ? $"{failed} ({string.Join(", ", operationResult.Errors)})"
                    : failed;
                    
                if (operationResult.Exception != null)
                    msg += $"\nException: {operationResult.Exception}";

                return msg;
            }
            default:
            {
                return operationResult.Errors?.Any() ?? false
                    ? $"{failed} ({string.Join(", ", operationResult.Errors)})"
                    : failed;
            }
        }
    }
}

public enum ReportLevel
{
    /// <summary> Only formed errors </summary>
    Default,

    /// <summary> Formed errors or exception message </summary>
    WithExceptionMessage,

    /// <summary> Formed errors with exception stack trace </summary>
    WithExceptionFull,
}