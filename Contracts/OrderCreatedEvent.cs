namespace Contracts;
public record OrderCreatedEvent(Guid OrderId, string Customer, DateTime CreatedDate, decimal OrderTotal, List<OrderItem> Items);
