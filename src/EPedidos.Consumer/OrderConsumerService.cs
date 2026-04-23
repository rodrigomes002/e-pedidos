using System.Text.Json;
using EPedidos.Shared;
using Microsoft.Extensions.Logging;

namespace EPedidos.Consumer;

public interface IOrderConsumerService
{
    Task ProcessOrderAsync(string jsonMessage, CancellationToken ct);
}

public sealed class OrderConsumerService : IOrderConsumerService
{
    private readonly OrdersDbContext _context;
    private readonly ILogger<OrderConsumerService> _logger;

    public OrderConsumerService(OrdersDbContext context, ILogger<OrderConsumerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ProcessOrderAsync(string jsonMessage, CancellationToken ct)
    {
        try
        {
            var orderEvent = JsonSerializer.Deserialize<OrderEvent>(jsonMessage);
            if (orderEvent == null)
            {
                _logger.LogWarning("Failed to deserialize order event");
                return;
            }

            var order = new Order
            {
                Id = orderEvent.OrderId,
                CustomerId = orderEvent.CustomerId,
                CustomerName = orderEvent.CustomerName,
                TotalAmount = orderEvent.TotalAmount,
                CreatedAt = orderEvent.CreatedAt,
                Items = orderEvent.Items
                    .Select(i => new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        Sku = i.Sku,
                        Description = i.Description,
                        UnitPrice = i.UnitPrice,
                        Quantity = i.Quantity
                    })
                    .ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Order {OrderId} saved to database", order.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order");
        }
    }
}
