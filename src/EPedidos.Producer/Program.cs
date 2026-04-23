using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using EPedidos.Producer;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Confluent.Kafka.Admin;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var kafkaBootstrap = context.Configuration["Kafka:BootstrapServers"] 
            ?? throw new InvalidOperationException("Missing Kafka bootstrap servers");
        
        var kafkaTopic = context.Configuration["Kafka:Topic"] ?? "orders";

        services.AddSingleton<IProducer<string, string>>(sp =>
        {
            var config = new ProducerConfig { BootstrapServers = kafkaBootstrap };
            return new ProducerBuilder<string, string>(config).Build();
        });

        services.AddSingleton<IAdminClient>(sp =>
        {
            var config = new AdminClientConfig { BootstrapServers = kafkaBootstrap };
            return new AdminClientBuilder(config).Build();
        });

        services.AddHostedService<OrderProducerWorker>();
    })
    .Build();

// Criar tópico antes de iniciar o host
using (var scope = host.Services.CreateScope())
{
    var adminClient = scope.ServiceProvider.GetRequiredService<IAdminClient>();
    var topicName = "orders";

    try
    {
        await adminClient.CreateTopicsAsync(new[]
        {
            new TopicSpecification
            {
                Name = topicName,
                NumPartitions = 1,
                ReplicationFactor = 1
            }
        });
        Console.WriteLine($"Topic '{topicName}' created successfully");
    }
    catch (CreateTopicsException ex) when (ex.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
    {
        Console.WriteLine($"Topic '{topicName}' already exists");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating topic: {ex.Message}");
    }
}

await host.RunAsync();
