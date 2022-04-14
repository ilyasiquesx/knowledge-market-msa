using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Logging;

public static class LoggerExtensions
{
    public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
            {
                IndexFormat = $"{Assembly.GetEntryAssembly()?.GetName().Name}-{environment.EnvironmentName}",
            })
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}")
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        return services;
    }
}