namespace Forum.API.Middlewares;

public class UnhandledExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UnhandledExceptionHandlerMiddleware> _logger;

    public UnhandledExceptionHandlerMiddleware(RequestDelegate next,
        ILogger<UnhandledExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            if (context.Response.HasStarted)
                return;

            _logger.LogError(e, "Global exception handling");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                e.Message
            });
        }
    }
}