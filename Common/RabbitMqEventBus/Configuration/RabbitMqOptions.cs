namespace RabbitMqEventBus.Configuration;

public interface IRabbitMqOptions
{
    public string Hostname { set; }
    public int Port { set; }
    public string Username { set; }
    public string Password { set; }
}

public class RabbitMqOptions : IRabbitMqOptions
{
    public string Hostname { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; }
    public string Password { get; set; }
    public int RetryCountToConnect { get; set; } = 3;
}

public class RabbitOptionsBase
{
    public string ExchangeName { get; set; }
}

public interface IRabbitPublisherOptions
{
    public string ExchangeName { set; }
}

public class RabbitPublisherOptions : RabbitOptionsBase, IRabbitPublisherOptions
{
}

public interface IRabbitHandlerOptions
{
    public string ExchangeName { set; }
    public string QueueName { set; }
}

public class RabbitHandlerOptions : RabbitOptionsBase, IRabbitHandlerOptions
{
    public string QueueName { get; set; }
}