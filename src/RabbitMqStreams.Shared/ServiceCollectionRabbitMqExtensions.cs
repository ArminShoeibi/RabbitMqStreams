using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace RabbitMqStreams.Shared;
public static class ServiceCollectionRabbitMqExtensions
{
    public static IServiceCollection AddRabbitMqAmqpConnection(this IServiceCollection services)
    {
        ConnectionFactory rabbitMqConnectionFactory = new()
        {
            TopologyRecoveryEnabled = true,
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
            RequestedHeartbeat = TimeSpan.FromSeconds(20),
        };

        // consider your try-catch and retry because of network and connection
        IConnection amqpConnection = rabbitMqConnectionFactory.CreateConnection();
        services.AddSingleton(amqpConnection);
        return services;
    }
}
