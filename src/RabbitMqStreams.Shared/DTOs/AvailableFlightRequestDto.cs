namespace RabbitMqStreams.Shared.DTOs;
public class AvailableFlightRequestDto
{
    public AvailableFlightRequestDto(string origin, string destination, DateTimeOffset departureDateTime)
    {
        ArgumentNullException.ThrowIfNull(origin);
        ArgumentNullException.ThrowIfNull(destination);

        Origin = origin;
        Destination = destination;
        DepartureDateTime = departureDateTime;
    }

    public string Origin { get; set; }
    public string Destination { get; set; }
    public DateTimeOffset DepartureDateTime { get; set; }
}
