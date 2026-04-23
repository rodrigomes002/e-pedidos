using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using System.Text.Json;
using EPedidos.Shared;

namespace EPedidos.Producer;

public sealed class OrderProducerWorker : BackgroundService
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<OrderProducerWorker> _logger;
    private readonly string _topic;

    public OrderProducerWorker(
        IProducer<string, string> producer,
        ILogger<OrderProducerWorker> logger)
    {
        _producer = producer;
        _logger = logger;
        _topic = "orders";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Producer worker starting...");

        // Gerar pedidos de exemplo a cada 1 milissegundo
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1));

        try
        {
            int orderCount = 0;
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var order = GenerateSampleOrder(orderCount++);
                await ProduceOrderAsync(order, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Producer worker stopped.");
        }
        finally
        {
            timer.Dispose();
        }
    }

    private async Task ProduceOrderAsync(OrderEvent order, CancellationToken ct)
    {
        try
        {
            var json = JsonSerializer.Serialize(order);
            var message = new Message<string, string>
            {
                Key = order.OrderId.ToString(),
                Value = json
            };

            var result = await _producer.ProduceAsync(_topic, message, ct);
            _logger.LogInformation("Order {OrderId} produced to partition {Partition} at offset {Offset}", 
                order.OrderId, result.Partition, result.Offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing order {OrderId}", order.OrderId);
        }
    }

    private OrderEvent GenerateSampleOrder(int count)
    {
        var customerId = Guid.Parse($"00000000-0000-0000-0000-{count:000000000000}");
        
        return new OrderEvent
        {
            OrderId = Guid.NewGuid(),
            CustomerId = customerId,
            CustomerName = $"Customer {count}",
            TotalAmount = Random.Shared.Next(100, 1000) * 10m,
            Items = new()
            {
                new OrderItemEvent
                {
                    Sku = $"SKU-{count:000}",
                    Description = $"Product {count}",
                    UnitPrice = Random.Shared.Next(10, 500),
                    Quantity = Random.Shared.Next(1, 5)
                }
            }
        };
    }
}
