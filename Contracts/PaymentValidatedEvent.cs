namespace Contracts;
public record PaymentValidatedEvent(Guid OrderId, List<OrderItem> Items);
