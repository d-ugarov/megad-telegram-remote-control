using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace MegaDTelegramRemoteControl.Infrastructure.Filters;

public class HttpResponseExceptionFilter : IActionFilter
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception == null)
            return;

        logger.Error(context.Exception);

        context.Result = new ObjectResult(context.Exception)
                         {
                             StatusCode = StatusCodes.Status500InternalServerError,
                         };
        context.ExceptionHandled = true;
    }
}