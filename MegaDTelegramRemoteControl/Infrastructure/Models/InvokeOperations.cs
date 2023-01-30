using NLog;
using System;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Infrastructure.Models;

public static class InvokeOperations
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public static OperationResult InvokeOperation(Action action)
    {
        try
        {
            action();
            return OperationResult.Ok();
        }
        catch (OperationException ex)
        {
            Log(ex);
            return OperationResult.Failed(ex.Message, ex, ex.ErrorCode, ex.ErrorData);
        }
        catch (Exception ex)
        {
            Log(ex);
            return OperationResult.Failed(ex);
        }
    }

    public static OperationResult<T> InvokeOperation<T>(Func<T> func)
    {
        try
        {
            var res = func();
            return OperationResult<T>.Ok(res);
        }
        catch (OperationException ex)
        {
            Log(ex);
            return OperationResult<T>.Failed(ex.Message, ex, ex.ErrorCode, ex.ErrorData);
        }
        catch (Exception ex)
        {
            Log(ex);
            return OperationResult<T>.Failed(ex);
        }
    }

    public static async Task<OperationResult> InvokeOperationAsync(Func<Task> func)
    {
        try
        {
            await func();
            return OperationResult.Ok();
        }
        catch (OperationException ex)
        {
            Log(ex);
            return OperationResult.Failed(ex.Message, ex, ex.ErrorCode, ex.ErrorData);
        }
        catch (Exception ex)
        {
            Log(ex);
            return OperationResult.Failed(ex);
        }
    }

    public static async Task<OperationResult<T>> InvokeOperationAsync<T>(Func<Task<T>> func)
    {
        try
        {
            var result = await func();
            return OperationResult<T>.Ok(result);
        }
        catch (OperationException ex)
        {
            Log(ex);
            return OperationResult<T>.Failed(ex.Message, ex, ex.ErrorCode, ex.ErrorData);
        }
        catch (Exception ex)
        {
            Log(ex);
            return OperationResult<T>.Failed(ex);
        }
    }

    private static void Log(Exception ex)
    {
        logger.Error(ex);
    }
}