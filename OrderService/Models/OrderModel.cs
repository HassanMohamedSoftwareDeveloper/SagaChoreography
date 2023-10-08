namespace OrderService.Models;

public record OrderModel(string Customer, List<OrderLineModel> OrderLines);
public record OrderLineModel(int ItemId, int Quantity, decimal Price);
