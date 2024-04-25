using ASB_Test.Infrastructure;
using ASB_Test.Services;

namespace ASB_Test;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        // Below we use an Environment Variable to store the Azure Service Bus Endpoint since we cannot check in those secrets to code repo
        string? azureServiceBusConnectionstring = Environment.GetEnvironmentVariable("AzureServiceBus");
        string queueOne = "queueone";
        string queueTwo = "abc";

        if (string.IsNullOrEmpty(azureServiceBusConnectionstring))
        {
            throw new ArgumentNullException("AzureServiceBus connection string is missing");
        }

        EnsureQueueExists(azureServiceBusConnectionstring, queueOne);
        EnsureQueueExists(azureServiceBusConnectionstring, queueTwo);

        builder.Services.AddTransient<QueueOneService>(s => new QueueOneService(azureServiceBusConnectionstring, queueOne));
        builder.Services.AddTransient<QueueTwoService>(s => new QueueTwoService(azureServiceBusConnectionstring, queueTwo));
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapPost("/QueueOne/SendMessage", async (string message) =>
        {
            QueueOneService queueOneService = app.Services.GetRequiredService<QueueOneService>();
            var bool_result = await queueOneService.SendMessage(message);
            return $"Send Result :: {bool_result}";
        }
        )
        .WithName("QueueOne/SendMessage")
        .WithSummary($"Queue = {queueOne}")
        .WithOpenApi();

        app.MapGet("/QueueOne/GetMessage", async () =>
        {
            QueueOneService queueOneService = app.Services.GetRequiredService<QueueOneService>();
            var body = await queueOneService.ReceiveMessage();
            return $"Message Body :: {body}";
        }
        )
        .WithName("QueueOne/GetMessage")
        .WithDescription("This is a test description")
        .WithSummary($"Queue = {queueOne}")
        .WithOpenApi();

        app.MapPost("/QueueTwo/SendMessage", async (string message) =>
        {
            QueueTwoService queueTwoService = app.Services.GetRequiredService<QueueTwoService>();
            var bool_result = await queueTwoService.SendMessage(message);
            return $"Send Result :: {bool_result}";
        }
        )
        .WithName("QueueTwo/SendMessage")
        .WithSummary($"Queue = {queueTwo}")
        .WithOpenApi();

        app.MapGet("/QueueTwo/GetMessage", async () =>
        {
            QueueTwoService queueTwoService = app.Services.GetRequiredService<QueueTwoService>();
            var body = await queueTwoService.ReceiveMessage();
            return $"Message Body :: {body}";
        }
        )
        .WithName("/QueueTwo/GetMessage")
        .WithSummary($"Queue = {queueTwo}")
        .WithOpenApi();

        app.Run();
    }

    private static void EnsureQueueExists(string connectionString, string queueName)
    {
        Queue queue = new Queue();
        queue.CreateQueueIfNotExists(connectionString, queueName).Wait();
    }
}
