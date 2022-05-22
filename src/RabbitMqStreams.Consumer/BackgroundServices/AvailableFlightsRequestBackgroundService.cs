using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RabbitMqStreams.Shared;
using RabbitMqStreams.Consumer.Consumers;

namespace RabbitMqStreams.Consumer.BackgroundServices;

// Note: each of the scenarios should be measured within the context of your application. 
internal class AvailableFlightsRequestBackgroundService : BackgroundService
{
    private readonly ILogger<AvailableFlightsRequestBackgroundService> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IConnection _amqpConnection;
    public AvailableFlightsRequestBackgroundService(ILogger<AvailableFlightsRequestBackgroundService> logger,
                                                    ILoggerFactory loggerFactory,
                                                    IConnection amqpConnection)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _amqpConnection = amqpConnection;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested is false)
        {
            try
            {
                using (var amqpChannel = _amqpConnection.CreateModel())
                {
                    amqpChannel.ExchangeDeclare(RabbitMqExchanges.AvailableFlightRequestDtoExchange, ExchangeType.Fanout, durable: true);

                    Dictionary<string, object> queueArgs = new();
                    queueArgs.Add(Headers.XQueueType, "stream"); // discard datas every 10 minutes.

                    //Streams are implemented as an immutable append-only disk log.
                    //This means that the log will grow indefinitely until the disk runs out.
                    //To avoid this undesirable scenario it is possible to set a retention configuration
                    //per stream which will discard the oldest data in the log based on total log data size and/or age.
                    queueArgs.Add("x-max-age", "10m"); // discard datas every 10 minutes.
                    amqpChannel.QueueDeclare(RabbitMqQueues.AvailableFlightRequestDtoQueue, true, false, false, queueArgs);

                    amqpChannel.QueueBind(RabbitMqQueues.AvailableFlightRequestDtoQueue, RabbitMqExchanges.AvailableFlightRequestDtoExchange, string.Empty);
                }

                // Imagine these are 4 separate services
                CreateStreamsConsumer("Consumer1");
                CreateStreamsConsumer("Consumer2");
                CreateStreamsConsumer("Consumer3");
                CreateStreamsConsumer("Consumer4");
                break;
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogCritical(ex, "Unable to open an AMQP connection for RabbitMQ");
            }
            catch (AlreadyClosedException ex)
            {
                _logger.LogCritical(ex, "Connection was opened but couldn't open a new AMQP channel for RabbitMQ");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "RabbitMQ has some connection issue");
            }
            await Task.Delay(2000);
        }
    }

    private void CreateStreamsConsumer(string loggerCategoryName)
    {
        IModel amqpChannel = _amqpConnection.CreateModel();
        AvailableFlightsRequestConsumer availableFlightsRequestConsumer = new(amqpChannel, _loggerFactory.CreateLogger(loggerCategoryName));
        amqpChannel.BasicQos(0, 1, false);
        amqpChannel.BasicConsume(RabbitMqQueues.AvailableFlightRequestDtoQueue, false, availableFlightsRequestConsumer);
    }
}