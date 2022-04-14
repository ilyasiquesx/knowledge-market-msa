using Forum.Core;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Forum.API;

public class DomainExceptionFilterAttribute : Attribute, IAsyncExceptionFilter
{
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is DomainException)
        {
            context.HttpContext.Response.StatusCode = 400;
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                context.Exception.Message
            });
        }
    }
}