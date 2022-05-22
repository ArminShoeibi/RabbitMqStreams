using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMqStreams.Publisher.Publishers;
using RabbitMqStreams.Shared;
using RabbitMqStreams.Shared.DTOs;
using System.Text.Json;

namespace RabbitMqStreams.Publisher.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AvailableFlightsController : ControllerBase
{
    private readonly RabbitMqPublisher _rabbitMqPublisher;

    public AvailableFlightsController(RabbitMqPublisher rabbitMqPublisher)
    {
       _rabbitMqPublisher = rabbitMqPublisher;
    }

    [HttpPost]
    public IActionResult PublishAvailableFlightsRequest(AvailableFlightRequestDto availableFlightRequestDto)
    {
        _rabbitMqPublisher.Publish(RabbitMqExchanges.AvailableFlightRequestDtoExchange,
                                   "",
                                   JsonSerializer.SerializeToUtf8Bytes(availableFlightRequestDto));

        return Ok();
    }
}
