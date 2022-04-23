using Forum.Core;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Forum.API.Filters;

public class DomainExceptionFilterAttribute : Attribute, IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is DomainException de)
        {
            var logger =
                context.HttpContext.RequestServices.GetRequiredService<ILogger<DomainExceptionFilterAttribute>>();
            logger.LogWarning(de, "{Message}", "Domain exception is thrown");
            context.HttpContext.Response.StatusCode = 400;
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                de.Message
            });
        }
    }
}