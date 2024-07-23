using OT.Assessment.Tester.Infrastructure;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace OT.Assessment.App.Services
{
    public class RabbitMQService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQService> _logger;

        public RabbitMQService(ILogger<RabbitMQService> logger)
        {
            _logger = logger;


            try
            {
                // Initialize the connection factory with RabbitMQ server details
                _factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };

                // Create a new connection to RabbitMQ and a new channel for communication
                _connection = _factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare a queue to ensure it exists before using it
                _channel.QueueDeclare(queue: "casino_wager_queue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            }
            catch (Exception ex)
            {

                // Log any exception that occurs during initialization
                _logger.LogError(ex, "Failed to initialize RabbitMQ connection and channel");
                throw new Exception("RabbitMQ initialization error", ex);
            }
        }

        public void PublishWager(CasinoWager wager)
        {
            try
            {
                var message = JsonSerializer.Serialize(wager);
                var body = Encoding.UTF8.GetBytes(message);

                // Publish the message to the RabbitMQ queue
                _channel.BasicPublish(exchange: "",
                                     routingKey: "casino_wager_queue",
                                     basicProperties: null,
                                     body: body);

                // Log the successful publishing of the wager
                _logger.LogInformation("Published wager: {WagerId}", wager.WagerId);
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during message publishing
                _logger.LogError(ex, "Failed to publish wager: {WagerId}", wager.WagerId);
                throw new Exception("RabbitMQ publish error", ex);
            }
        }

        public void Dispose()
        {
            try
            {

                // Safely close the channel and connection
                _channel?.Close();
                _connection?.Close();
                _logger.LogInformation("RabbitMQ connection and channel closed successfully");
            }
            catch (Exception ex)
            {
                // Log any exception that occurs during closure
                _logger.LogError(ex, "Failed to close RabbitMQ connection and channel");
            }
        }
    }
}
