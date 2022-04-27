using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Forum.API.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .Where(v => v.Errors.Count > 0)
                .SelectMany(e => e.Errors)
                .Select(c => c.ErrorMessage);

            var badRequestObjectResult = new BadRequestObjectResult(new
            {
                errors
            });

            context.Result = badRequestObjectResult;
            return;
        }

        await next();
    }
}