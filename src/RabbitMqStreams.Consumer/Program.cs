using RabbitMqStreams.Consumer.BackgroundServices;
using RabbitMqStreams.Shared;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddRabbitMqAmqpConnection();
        services.AddHostedService<AvailableFlightsRequestBackgroundService>();
    })
    .Build();

await host.RunAsync();
