using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Models;
using System.Text;
using System.Text.Json;

namespace PlatformService.Services.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _factory;
        private IConnection? _connection;
        private IChannel? _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;

            _factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"] ?? "localhost",
                Port = _configuration.GetValue<int>("RabbitMQPort", 5672)
            };
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

                await _channel.ExchangeDeclareAsync(
                    exchange: "trigger",
                    type: ExchangeType.Fanout,
                    cancellationToken: cancellationToken);

                _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdownAsync;

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the MessageBus: {ex.Message}");
            }
        }

        public async Task PublishNewPlatformAsync(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);

            if (_connection != null && _connection.IsOpen)
            {
                Console.WriteLine("--> MessageBus Connection Open, sending message...");
                await SendMessageAsync(message);
            }
            else
            {
                Console.WriteLine("--> MessageBus Connection Closed, not sending");
            }
        }

        public async ValueTask DisposeAsync()
        {
            Console.WriteLine("MessageBus Disposed");

            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
            }

            if (_connection != null && _connection.IsOpen)
            {
                await _connection.CloseAsync();
            }
        }

        private async Task SendMessageAsync(string message)
        {
            if (_channel != null)
            {
                var body = Encoding.UTF8.GetBytes(message);

                await _channel.BasicPublishAsync(
                    exchange: "trigger",
                    routingKey: "",
                    body: body);

                Console.WriteLine($"--> We have sent {message}");
            }
        }

        private async Task RabbitMQ_ConnectionShutdownAsync(object sender, ShutdownEventArgs @event)
        {
            Console.WriteLine("--> MessageBus Connection Shutdown");
        }
    }
}
