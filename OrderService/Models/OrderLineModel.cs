namespace OrderService.Models;

public record OrderLineModel(int ItemId, int Quantity, decimal Price);
