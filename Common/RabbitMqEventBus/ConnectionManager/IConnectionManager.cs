using RabbitMQ.Client;

namespace RabbitMqEventBus.ConnectionManager;

internal interface IConnectionManager : IDisposable
{
    IModel CreateModel();
}