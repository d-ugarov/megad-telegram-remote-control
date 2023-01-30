using System;

namespace MegaDTelegramRemoteControl.Infrastructure.Models;

public class OperationException : Exception
{
    public ErrorCodes ErrorCode { get; }
    public object? ErrorData { get; }

    public OperationException(ErrorCodes errorCode = default, object? errorData = null)
    {
        ErrorCode = errorCode;
        ErrorData = errorData;
    }

    public OperationException(string message, ErrorCodes errorCode = default, object? errorData = null)
        : base(message)
    {
        ErrorCode = errorCode;
        ErrorData = errorData;
    }

    public OperationException(string message, Exception inner, ErrorCodes errorCode = default, object? errorData = null)
        : base(message, inner)
    {
        ErrorCode = errorCode;
        ErrorData = errorData;
    }
}