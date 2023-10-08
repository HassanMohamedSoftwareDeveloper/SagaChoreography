namespace Contracts;

public record InventoryReservedEvent(Guid OrderId, string Customer, decimal Total, List<OrderItem> Items);
