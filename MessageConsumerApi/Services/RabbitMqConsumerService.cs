using MessageConsumerApi.Services.IServices;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;


namespace MessageConsumerApi.Services
{
    public class RabbitMqConsumerService : IRabbitMqConsumerService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "trackingNumbersQueue";
        private readonly ILogger<RabbitMqConsumerService> _logger;
        private bool _disposed;

        public RabbitMqConsumerService(
            IConfiguration configuration,
            ILogger<RabbitMqConsumerService> logger)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Service starting...");

            cancellationToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message: {Message}", message);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            _logger.LogInformation("RabbitMQ Consumer Service started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ Consumer Service stopping...");
            Dispose();
            _logger.LogInformation("RabbitMQ Consumer Service stopped");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}