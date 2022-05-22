using RabbitMQ.Client;

namespace RabbitMqStreams.Publisher.Publishers;

public class RabbitMqPublisher
{
    private readonly IConnection _amqpConnection;
    private readonly object publishLock = new object();
    private readonly IModel _amqpChannel;
    public RabbitMqPublisher(IConnection amqpConnection)
    {
        _amqpConnection = amqpConnection;
        _amqpChannel = _amqpConnection.CreateModel(); // consider network and retry policy (this is not a production code)
    }
    public void Publish(string exchangeName, string routingKey, byte[] data)
    {
        lock (publishLock)
        {
            _amqpChannel.BasicPublish(exchangeName, routingKey, null, data);
        }
    }
}
