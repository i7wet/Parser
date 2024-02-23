using RabbitMQ.Client;

namespace RabbitMQ;

public class RabbitMq
{
    public static IModel CreateChannel()
    {
        ConnectionFactory factory = new ConnectionFactory();

        factory.HostName = "localhost";
        factory.Port = 5672;

        IConnection conn = factory.CreateConnection();
        var channel = conn.CreateModel();
        return channel;
    }
}