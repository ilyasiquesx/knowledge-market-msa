using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqEventBus.Configuration;
using RabbitMqEventBus.ConnectionManager;

namespace RabbitMqEventBus.MessageHandler;

internal sealed class MessageHandler : IMessageHandler
{
    private readonly ILogger<MessageHandler> _logger;

    private IModel _model;
    private readonly IConnectionManager _connectionManager;
    private IMessageCallback _callback;

    private readonly string _queueName;
    private readonly string _exchangeName;

    public MessageHandler(RabbitHandlerOptions handlerOptions,
        ILogger<MessageHandler> logger,
        IConnectionManager connectionManager)
    {
        _logger = logger;
        _connectionManager = connectionManager;

        var options = handlerOptions ?? throw new ArgumentNullException(nameof(handlerOptions));
        _exchangeName = options.ExchangeName;
        _queueName = options.QueueName;

        _model = _connectionManager.CreateModel();
    }

    public void StartConsume(IMessageCallback callback)
    {
        _logger.LogInformation("{Message} {AssemblyName}",
            "Setting callback for message handler and starting consuming queue",
            Assembly.GetEntryAssembly()?.GetName().Name);

        _callback = callback;
        DeclareQueueAndConsume(_queueName, _exchangeName);
    }

    private void DeclareQueueAndConsume(string queueName, string exchangeName)
    {
        _model.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: true,
            autoDelete: false);
        _model.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);
        _model.QueueBind(queueName, exchangeName, string.Empty);
        var consumer = new AsyncEventingBasicConsumer(_model);
        consumer.Received += ConsumerOnReceived;
        consumer.Shutdown += ConsumerOnShutdown;
        consumer.Registered += ConsumerOnRegistered;
        _model.BasicConsume(queueName, false, consumer);
    }

    private async Task ConsumerOnReceived(object _, BasicDeliverEventArgs ea)
    {
        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
        var headers = ea.BasicProperties.Headers ?? new Dictionary<string, object>();
        var decodedHeaders = headers.ToDictionary(kvp => kvp.Key,
            values => Encoding.UTF8.GetString((byte[])values.Value));
        var serializedHeaders = JsonConvert.SerializeObject(decodedHeaders);
        var routingKey = ea.RoutingKey;

        _logger.LogInformation("{Message} {RoutingKey} {MessageHeaders} {Body}", "Received message to handle",
            routingKey,
            serializedHeaders, message);

        if (await HandleMessage(routingKey, message))
        {
            _logger.LogInformation("{Message} {RoutingKey} {MessageHeaders} {Body}",
                "Message handled successful. Doing ack",
                routingKey,
                serializedHeaders, message);

            _model.BasicAck(ea.DeliveryTag, false);
            return;
        }

        _logger.LogWarning("{Message} {RoutingKey} {MessageBody}", "Cannot handle a message. Rejecting...", routingKey,
            message);
        _model.BasicReject(ea.DeliveryTag, false);
    }

    private async Task<bool> HandleMessage(string messageType, string message)
    {
        try
        {
            await _callback.Invoke(messageType, message);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Message} {MessageType} {MessageBody}", "Message handling error", messageType,
                message);
            return false;
        }
    }

    private Task ConsumerOnShutdown(object sender, ShutdownEventArgs @event)
    {
        _logger.LogWarning("{Message} {ReplyText}", "Consumer shut down. Recreating consumer channel...",
            @event.ReplyText);
        _model?.Dispose();
        _model = _connectionManager.CreateModel();
        DeclareQueueAndConsume(_queueName, _exchangeName);
        return Task.CompletedTask;
    }

    private Task ConsumerOnRegistered(object sender, ConsumerEventArgs @event)
    {
        _logger.LogInformation("{Message} {@EventArgs}", "Consumer registered successful", @event);
        return Task.CompletedTask;
    }
}