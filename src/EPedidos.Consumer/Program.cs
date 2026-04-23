using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EPedidos.Consumer;
using Confluent.Kafka;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("Postgres") 
            ?? throw new InvalidOperationException("Missing connection string 'Postgres'");
        
        var kafkaBootstrap = context.Configuration["Kafka:BootstrapServers"] 
            ?? throw new InvalidOperationException("Missing Kafka bootstrap servers");
        
        var kafkaTopic = context.Configuration["Kafka:Topic"] ?? "orders";

        services.AddDbContext<OrdersDbContext>(opt =>
            opt.UseNpgsql(connectionString));

        services.AddSingleton<IConsumer<string, string>>(sp =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaBootstrap,
                GroupId = "order-consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };
            return new ConsumerBuilder<string, string>(config).Build();
        });

        services.AddScoped<IOrderConsumerService, OrderConsumerService>();
        services.AddHostedService<OrderConsumerWorker>();
    })
    .Build();

// Aplicar migrations ao iniciar
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await db.Database.EnsureCreatedAsync();
}

await host.RunAsync();
