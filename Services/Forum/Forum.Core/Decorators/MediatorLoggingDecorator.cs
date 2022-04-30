using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace Forum.Core.Decorators;

public class MediatorLoggingDecorator : Mediator, IMediator
{
    private readonly IMediator _mediator;
    private readonly ILogger<MediatorLoggingDecorator> _logger;

    public MediatorLoggingDecorator(ServiceFactory serviceFactory, IMediator mediator,
        ILogger<MediatorLoggingDecorator> logger) : base(serviceFactory)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public new async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var type = request.GetType().Name;
        _logger.LogInformation("{Message} {RequestType} {@RequestItem}",
            "The request has started",
            type,
            request);

        var sw = new Stopwatch();

        sw.Start();
        var result = await _mediator.Send(request, cancellationToken);
        sw.Stop();

        object responseItem = result;
        if (result is IOneOf oneOf)
        {
            responseItem = oneOf.Value;
        }

        var responseItemType = responseItem.GetType().Name;
        _logger.LogInformation("{Message} {RequestType} {@RequestBody} {ResponseType} {@ResponseItem} {MillisecondsSpent}",
            $"The request has finished within {sw.ElapsedMilliseconds} ms",
            type,
            request,
            responseItemType,
            responseItem,
            sw.ElapsedMilliseconds);

        return result;
    }
}