using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMqEventBus.Configuration;
using RabbitMqEventBus.ConnectionManager;
using RabbitMqEventBus.HandlerManager;
using RabbitMqEventBus.MessageHandler;
using RabbitMqEventBus.MessagePublisher;

namespace RabbitMqEventBus.DependencyInjection;

public static class RabbitMqExtensions
{
    /// <summary>
    /// Register Rabbit Connection using appSettings configuration
    /// </summary>
    /// <param name="services">Current container</param>
    /// <param name="configuration">Configuration object</param>
    /// <param name="sectionName">AppSettings section name for connection manager</param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitConnection(this IServiceCollection services, IConfiguration configuration,
        string sectionName)
    {
        var section = configuration.GetSection(sectionName);
        var rabbitMqOptions = new RabbitMqOptions();
        section.Bind(rabbitMqOptions);
        services.AddSingleton(rabbitMqOptions);
        services.AddSingleton<IConnectionManager, ConnectionManager.ConnectionManager>();
        return services;
    }

    /// <summary>
    /// Register Rabbit Connection using Action
    /// </summary>
    /// <param name="services">Current container</param>
    /// <param name="configureOptions">Action to configure connection manager</param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitConnection(this IServiceCollection services,
        Action<IRabbitMqOptions> configureOptions)
    {
        var opts = new RabbitMqOptions();
        configureOptions(opts);
        services.AddSingleton(opts);
        services.AddSingleton<IConnectionManager, ConnectionManager.ConnectionManager>();
        return services;
    }

    /// <summary>
    /// Register Message Handler in your project
    /// </summary>
    /// <param name="serviceCollection">Current container</param>
    /// <param name="configuration">Configuration object</param>
    /// <param name="sectionName">AppSettings section name for handler (ExchangeName, QueueName)</param>
    /// <returns></returns>
    public static IServiceCollection AddMessageHandler(this IServiceCollection serviceCollection,
        IConfiguration configuration, string sectionName)
    {
        var handlerOptions = new RabbitHandlerOptions();
        configuration.GetSection(sectionName).Bind(handlerOptions);
        serviceCollection.AddSingleton(handlerOptions);

        serviceCollection.AddSingleton<IMessageHandler, MessageHandler.MessageHandler>();
        return serviceCollection;
    }

    /// <summary>
    /// Register Message Handler using Action
    /// </summary>
    /// <param name="serviceCollection">Current container</param>
    /// <param name="configureOptions">Action to configure message handler</param>
    /// <returns></returns>
    public static IServiceCollection AddMessageHandler(this IServiceCollection serviceCollection,
        Action<IRabbitHandlerOptions> configureOptions)
    {
        var handlerOptions = new RabbitHandlerOptions();
        configureOptions(handlerOptions);
        serviceCollection.AddSingleton(handlerOptions);

        serviceCollection.AddSingleton<IMessageHandler, MessageHandler.MessageHandler>();
        return serviceCollection;
    }

    /// <summary>
    /// Register Handler Manager
    /// </summary>
    /// <param name="serviceCollection">Current container</param>
    /// <param name="configureManager">Action to configure handler manager</param>
    /// <returns></returns>
    public static IServiceCollection AddHandlerManager(this IServiceCollection serviceCollection,
        Action<IHandlerManagerConfigurator> configureManager)
    {
        serviceCollection.AddSingleton(configureManager);
        serviceCollection.AddSingleton<IHandlerManager, HandlerManager.HandlerManager>();
        return serviceCollection;
    }

    /// <summary>
    /// Register message publisher
    /// </summary>
    /// <param name="serviceCollection">Current container</param>
    /// <param name="configuration">Configuration object</param>
    /// <param name="sectionName">AppSettings section name for message publisher</param>
    /// <returns></returns>
    public static IServiceCollection AddMessagePublisher(this IServiceCollection serviceCollection,
        IConfiguration configuration, string sectionName)
    {
        var publisherOptions = new RabbitPublisherOptions();
        configuration.GetSection(sectionName).Bind(publisherOptions);
        serviceCollection.AddSingleton(publisherOptions);
        serviceCollection.AddTransient<IMessagePublisher, MessagePublisher.MessagePublisher>();
        return serviceCollection;
    }

    /// <summary>
    /// Register message publisher
    /// </summary>
    /// <param name="serviceCollection">Current container</param>
    /// <param name="configureOptions">Action to configure message publisher</param>
    /// <returns></returns>
    public static IServiceCollection AddMessagePublisher(this IServiceCollection serviceCollection,
        Action<IRabbitPublisherOptions> configureOptions)
    {
        var publisherOptions = new RabbitPublisherOptions();
        configureOptions(publisherOptions);
        serviceCollection.AddSingleton(publisherOptions);
        serviceCollection.AddTransient<IMessagePublisher, MessagePublisher.MessagePublisher>();
        return serviceCollection;
    }
}