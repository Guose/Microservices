using CommandService.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace CommandService.Services.Subscriber
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection? _connection;
        private IChannel? _channel;
        private string _queueName = string.Empty;

        public MessageBusSubscriber(
            IConfiguration configuration,
            IEventProcessor eventProcessor)
        {
            _configuration = configuration;
            _eventProcessor = eventProcessor;

            InitializeRabbitMQ().GetAwaiter().GetResult();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(_channel!);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                Console.WriteLine("--> Event Received!");
                var body = ea.Body.ToArray();
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                await _eventProcessor.ProcessEventAsync(notificationMessage);
            };

            await _channel!.BasicConsumeAsync(
                queue: _queueName,
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken);
        }

        private async Task InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"] ?? "localhost",
                Port = _configuration.GetValue<int>("RabbitMQPort", 5672)
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = (await _channel.QueueDeclareAsync()).QueueName;
            await _channel.QueueBindAsync(
                queue: _queueName,
                exchange: "trigger",
                routingKey: string.Empty);

            Console.WriteLine("--> Listening on the Message Bus...");

            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdownAsync;
        }

        private async Task RabbitMQ_ConnectionShutdownAsync(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine("--> RabbitMQ connection shut down");
            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel!.IsOpen)
            {
                _channel.CloseAsync();
                _connection!.CloseAsync();
            }

            base.Dispose();
        }
    }
}
