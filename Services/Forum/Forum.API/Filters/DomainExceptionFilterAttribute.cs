using Forum.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Forum.API.Filters;

public class DomainExceptionFilterAttribute : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is not DomainException de)
            return Task.CompletedTask;

        var logger =
            context.HttpContext.RequestServices.GetRequiredService<ILogger<DomainExceptionFilterAttribute>>();
        logger.LogWarning(de, "{Message}", "Domain exception is thrown");

        context.Result = new BadRequestObjectResult(new
        {
            de.Message
        });

        return Task.CompletedTask;
    }
}