using Microsoft.Extensions.Configuration;
using OT.Assessment.Consumer;
using OT.Assessment.Consumer.Settings;
using OT.Assessments.Modules.CasinoWagerRepository;
using OT.Assessments.Modules.PlayerAccountRepository;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    })
    .ConfigureServices((context, services) =>
    {
        // Register the configuration sections
        services.Configure<DatabaseSettings>(context.Configuration.GetSection("ConnectionStrings"));
        services.Configure<RabbitMQSettings>(context.Configuration.GetSection("RabbitMQ"));

        // Register repositories
        services.AddScoped<IPlayerAccountRepository, PlayerAccountRepository>();
        services.AddScoped<ICasinoWagerRepository, CasinoWagerRepository>();

        // Register the RabbitMQ consumer service
        services.AddHostedService<RabbitMQConsumerService>();

    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

await host.RunAsync();

logger.LogInformation("Application ended {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);