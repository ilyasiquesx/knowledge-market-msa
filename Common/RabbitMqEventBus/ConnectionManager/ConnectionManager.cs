using System.Reflection;
using Microsoft.Extensions.Logging;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqEventBus.Configuration;

namespace RabbitMqEventBus.ConnectionManager;

internal sealed class ConnectionManager : IConnectionManager
{
    private readonly RabbitMqOptions _connectionOptions;
    private readonly ILogger<ConnectionManager> _logger;
    private IConnection _connection;
    private bool _isDisposed;

    public ConnectionManager(RabbitMqOptions connectionOptions, ILogger<ConnectionManager> logger)
    {
        _logger = logger;
        _connectionOptions = connectionOptions ?? throw new ArgumentNullException(nameof(connectionOptions));
        _connection = CreateConnection(_connectionOptions);
    }

    public IModel CreateModel()
    {
        return GetConnection().CreateModel();
    }

    private IConnection GetConnection()
    {
        if (_connection is { IsOpen: true })
            return _connection;

        _logger.LogInformation("Connection is closed. Reconnecting...");
        _connection?.Dispose();
        _connection = CreateConnection(_connectionOptions);
        return _connection;
    }

    private IConnection CreateConnection(RabbitMqOptions options)
    {
        var connectionFactory = new ConnectionFactory
        {
            UserName = options.Username,
            Password = options.Password,
            HostName = options.Hostname,
            Port = options.Port,
            DispatchConsumersAsync = true,
            ClientProvidedName = Assembly.GetEntryAssembly()?.GetName().Name
        };

        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetry(options.RetryCountToConnect, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (exception, span, attempt, context) =>
                {
                    _logger.LogWarning(exception,
                        "{Message} {TimeSpan} {Attempt}",
                        "Retrying to reconnect after exception",
                        span, attempt);
                });

        var connection = policy.Execute(() => connectionFactory.CreateConnection());
        connection.CallbackException += ConnectionOnCallbackException;
        connection.ConnectionShutdown += ConnectionOnConnectionShutdown;

        return connection;
    }

    private void ConnectionOnConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogWarning("{Message} {@EventArgs}", "Connection is shut down", e);
    }

    private void ConnectionOnCallbackException(object sender, CallbackExceptionEventArgs e)
    {
        _logger.LogWarning(e.Exception, "Connection thrown an exception");
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        _isDisposed = true;

        try
        {
            _connection.CallbackException -= ConnectionOnCallbackException;
            _connection.ConnectionShutdown -= ConnectionOnConnectionShutdown;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while disposing connection manager");
        }

        _connection?.Close();
        _connection?.Dispose();
    }
}