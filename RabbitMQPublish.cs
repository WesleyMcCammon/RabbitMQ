using System.Text;
using RabbitMQ.Client;

namespace Messaging.RabbitMQ
{
    public static class RabbitMQPublish
    {
        private static readonly string HOSTNAME = "localhost";

        public static void Send(string queueName, string message)
        {
            var factory = new ConnectionFactory() { HostName = HOSTNAME };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
            }
        }
    }
}