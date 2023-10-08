namespace Contracts;

public record PaymentFailedEvent(Guid OrderId, List<OrderItem> Items);
