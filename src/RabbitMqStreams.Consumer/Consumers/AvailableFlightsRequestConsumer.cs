using Microsoft.Toolkit.HighPerformance;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMqStreams.Shared.DTOs;
using System.Text.Json;

namespace RabbitMqStreams.Consumer.Consumers;
internal class AvailableFlightsRequestConsumer : AsyncEventingBasicConsumer
{
    private readonly ILogger _logger;

    public AvailableFlightsRequestConsumer(IModel model, ILogger logger) : base(model)
    {
        _logger = logger;
    }

    public override async Task HandleBasicDeliver(string consumerTag,
                                                  ulong deliveryTag,
                                                  bool redelivered,
                                                  string exchange,
                                                  string routingKey,
                                                  IBasicProperties properties,
                                                  ReadOnlyMemory<byte> body)
    {
        var availableFlightRequestDto = await JsonSerializer.DeserializeAsync<AvailableFlightRequestDto>(body.AsStream());
        _logger.LogInformation("AvailableRequest: {Origin} {Destination} {DepartureDateTime}",
                               availableFlightRequestDto.Origin,
                               availableFlightRequestDto.Destination,
                               availableFlightRequestDto.DepartureDateTime);

        Model.BasicAck(deliveryTag, false);
    }
}
