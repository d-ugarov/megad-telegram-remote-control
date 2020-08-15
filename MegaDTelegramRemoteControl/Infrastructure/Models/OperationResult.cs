using System;
using System.Collections.Generic;
using System.Linq;

namespace MegaDTelegramRemoteControl.Infrastructure.Models
{
    public interface IOperationResult
    {
        bool IsSuccess { get; }
        List<string> Errors { get; }
        Exception Exception { get; }

        void EnsureSuccess();
    }

    public class OperationResult : IOperationResult
    {
        public bool IsSuccess { get; private set; }
        public List<string> Errors { get; private set; }
        public Exception Exception { get; private set; }

        public void EnsureSuccess()
        {
            if (IsSuccess)
                return;
            
            if (Exception != null)
                throw Exception;

            if (Errors != null && Errors.Any())
                throw new Exception(this.Report());
            
            throw new Exception($"Unknown error");
        }
        
        public static OperationResult Ok()
        {
            return new OperationResult
                   {
                       IsSuccess = true,
                       Errors = new List<string>()
                   };
        }

        public static OperationResult Failed(Exception exception = null)
        {
            return new OperationResult
                   {
                       IsSuccess = false,
                       Errors = new List<string>(),
                       Exception = exception
                   };
        }

        public static OperationResult Failed(string error, Exception exception = null)
        {
            return new OperationResult
                   {
                       IsSuccess = false,
                       Errors = new List<string> {error},
                       Exception = exception
                   };
        }

        public static OperationResult Failed(IEnumerable<string> errors, Exception exception = null)
        {
            return new OperationResult
                   {
                       IsSuccess = false,
                       Errors = errors.ToList(),
                       Exception = exception
                   };
        }
    }

    public class OperationResult<T> : IOperationResult
    {
        public bool IsSuccess { get; private set; }
        public List<string> Errors { get; private set; }
        public Exception Exception { get; private set; }
        public T Data { get; set; }

        public T DataUnsafe
        {
            get
            {
                if (IsSuccess)
                    return Data;
                
                if (Exception != null)
                    throw Exception;

                if (Errors.Any())
                    throw new Exception(this.Report());
                
                throw new Exception($"Unknown error");
            }
        }

        public OperationResult ActionResult => IsSuccess ? OperationResult.Ok() : OperationResult.Failed(Errors, Exception);

        public void EnsureSuccess()
        {
            if (IsSuccess)
                return;
            
            if (Exception != null)
                throw Exception;

            if (Errors.Any())
                throw new Exception(this.Report());
            
            throw new Exception($"Unknown error");
        }
        
        public static OperationResult<T> Ok()
        {
            return new OperationResult<T>
                   {
                       IsSuccess = true,
                       Errors = new List<string>()
                   };
        }

        public static OperationResult<T> Ok(T result)
        {
            return new OperationResult<T>
            {
                       IsSuccess = true,
                       Errors = new List<string>(),
                       Data = result
                   };
        }

        public static OperationResult<T> Failed(Exception exception = null)
        {
            return new OperationResult<T>
                   {
                       IsSuccess = false,
                       Errors = new List<string>(),
                       Exception = exception
                   };
        }

        public static OperationResult<T> Failed(string error, Exception exception = null)
        {
            return new OperationResult<T>
                   {
                       IsSuccess = false,
                       Errors = new List<string> {error},
                       Exception = exception
                   };
        }

        public static OperationResult<T> Failed(IEnumerable<string> errors, Exception exception = null)
        {
            return new OperationResult<T>
                   {
                       IsSuccess = false,
                       Errors = errors.ToList(),
                       Exception = exception
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
                    return operationResult.Errors.Any()
                        ? $"{failed} ({string.Join(", ", operationResult.Errors)})"
                        : !string.IsNullOrEmpty(operationResult.Exception?.Message)
                            ? $"{failed} ({operationResult.Exception.Message})"
                            : failed;
                }
                case ReportLevel.WithExceptionFull:
                {
                    var msg = operationResult.Errors.Any() ? $"{failed} ({string.Join(", ", operationResult.Errors)})" : failed;
                    
                    if (operationResult.Exception != null)
                        msg += $"\nException: {operationResult.Exception}";

                    return msg;
                }
                default:
                {
                    return operationResult.Errors.Any() ? $"{failed} ({string.Join(", ", operationResult.Errors)})" : failed;
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
}