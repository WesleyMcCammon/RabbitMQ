using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging.RabbitMQ {
    public delegate void EventHandler(string a); 

    public class RabbitMQConsumer : IDisposable {
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
        public event EventHandler MessageEventHandler = default!; 

        public RabbitMQConsumer(string queueName)
        {
            _connectionFactory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = queueName;
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            Console.WriteLine(String.Format("Starting {0}", _queueName));
            _channel.QueueDeclare(queue: _queueName, durable: false, 
                exclusive: false, autoDelete: false, arguments: null);
            Task.Run(() => {
                ReceiveMessages();
            });
        }
        public void ReceiveMessages()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) => {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                MessageEventHandler(message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }
    }
}