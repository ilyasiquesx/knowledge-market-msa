using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMqEventBus.Configuration;
using RabbitMqEventBus.ConnectionManager;

namespace RabbitMqEventBus.MessagePublisher;

internal sealed class MessagePublisher : IMessagePublisher
{
    private readonly string _exchangeName;
    private readonly ILogger<MessagePublisher> _logger;
    private readonly IConnectionManager _connectionManager;

    public MessagePublisher(RabbitPublisherOptions publisherOptions,
        ILogger<MessagePublisher> logger,
        IConnectionManager connectionManager)
    {
        _logger = logger;
        _connectionManager = connectionManager;
        _exchangeName = publisherOptions?.ExchangeName ?? throw new ArgumentNullException(nameof(publisherOptions));
    }

    public Task PublishAsync(string messageType, object message)
    {
        try
        {
            using var channel = _connectionManager.CreateModel();
            channel.ExchangeDeclare(_exchangeName, ExchangeType.Fanout, durable: true,
                autoDelete: false);
            channel.ConfirmSelect();
            var props = channel.CreateBasicProperties();
            var jsonString = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(jsonString);

            channel.BasicPublish(_exchangeName, messageType, props, body);
            channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(20));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Message publishing error");
        }
        finally
        {
            _logger.LogInformation("{Message} {Type} {Body}", "Sent message to the event bus", messageType,
                JsonConvert.SerializeObject(message));
        }

        return Task.CompletedTask;
    }
}