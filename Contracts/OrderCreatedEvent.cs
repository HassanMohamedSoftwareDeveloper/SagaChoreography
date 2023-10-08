namespace Contracts;
public record OrderCreatedEvent(Guid OrderId, DateTime CreatedDate, decimal OrderTotal, List<OrderItem> Items);
