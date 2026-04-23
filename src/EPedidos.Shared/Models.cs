namespace EPedidos.Shared;

public record OrderEvent
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public List<OrderItemEvent> Items { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

public record OrderItemEvent
{
    public string Sku { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}
