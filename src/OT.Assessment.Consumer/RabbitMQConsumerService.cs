using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OT.Assessment.Consumer.Settings;
using OT.Assessment.Models;
using OT.Assessments.Modules.PlayerAccountRepository;
using OT.Assessments.Modules.CasinoWagerRepository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OT.Assessment.Consumer
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _rabbitMQSettings;
        private readonly ILogger<RabbitMQConsumerService> _logger;
        private IConnection _connection;

        public RabbitMQConsumerService(IServiceProvider serviceProvider, IOptions<RabbitMQSettings> rabbitMQSettings, ILogger<RabbitMQConsumerService> logger)
        {
            _serviceProvider = serviceProvider;
            _rabbitMQSettings = rabbitMQSettings.Value;
            _logger = logger;

            try
            {
                // Initialize the RabbitMQ connection factory
                var factory = new ConnectionFactory()
                {
                    HostName = _rabbitMQSettings.HostName,
                    UserName = _rabbitMQSettings.UserName,
                    Password = _rabbitMQSettings.Password
                };

                // Create RabbitMQ connection and channel
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare the queue to ensure it exists before consuming messages
                _channel.QueueDeclare(queue: _rabbitMQSettings.QueueName,
                                      durable: false,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ connection and channel");
                throw new Exception("RabbitMQ initialization error", ex);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var wager = JsonSerializer.Deserialize<CasinoWager>(message);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var playerAccountRepository = scope.ServiceProvider.GetRequiredService<IPlayerAccountRepository>();
                        var casinoWagerRepository = scope.ServiceProvider.GetRequiredService<ICasinoWagerRepository>();


                        // Retrieve or create the player account
                        var playerAccount = await playerAccountRepository.GetPlayerAccountByIdAsync(new Guid(wager.accountId));
                        if (playerAccount == null)
                        {
                            playerAccount = new PlayerAccount
                            {
                                AccountId = new Guid(wager.accountId),
                                Username = wager.Username
                            };
                            await playerAccountRepository.AddPlayerAccountAsync(playerAccount);
                        }

                        await casinoWagerRepository.AddCasinoWagerAsync(wager);
                    }

                    // Manually acknowledge the message as processed
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    // Log the exception and negatively acknowledge the message to requeue it
                    _logger.LogError(ex, "Failed to process received message");
                    _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            // Start consuming messages from the queue
            _channel.BasicConsume(queue: _rabbitMQSettings.QueueName,
                                  autoAck: false,
                                  consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
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
                _logger.LogError(ex, "Failed to close RabbitMQ connection and channel");
            }

            base.Dispose();
        }
    }
}
