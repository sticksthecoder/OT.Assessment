using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OT.Assessment.App.Repositories;
using OT.Assessment.App.Services;
using OT.Assessment.App.SQLConnection;
using OT.Assessment.Consumer.Settings;
using OT.Assessments.Modules.PlayerAccountRepository;
using System.Data.SqlClient;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Ensure configuration sources are loaded
builder.Configuration
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();


// Add logging for diagnostics
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add controllers
builder.Services.AddControllers();

// Add services to app
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddSingleton<ISqlDatabaseConnection>(new SqlDatabaseConnection(builder.Configuration.GetConnectionString("DatabaseConnection")));

// Configure RabbitMQ settings
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

// Register repositories and services
builder.Services.AddScoped<IPlayerAccountRepository, PlayerAccountRepository>();
builder.Services.AddScoped<ICasinoWagerRepository, CasinoWagerRepository>();
builder.Services.AddScoped<ICasinoWagerService, CasinoWagerService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

try
{
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(opts =>
        {
            opts.EnableTryItOutByDefault();
            opts.DocumentTitle = "OT Assessment App";
            opts.DisplayRequestDuration();
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Exception during build: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    throw;
}
