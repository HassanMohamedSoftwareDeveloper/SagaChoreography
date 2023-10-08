namespace Contracts;

public record InventoryReleasedEvent(Guid OrderId, List<OrderItem> Items);
