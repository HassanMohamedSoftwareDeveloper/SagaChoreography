namespace OrderService.Models;

public record OrderModel(string Customer, decimal OrderTotal, string Status, DateTime OrderDate, List<OrderLineModel> OrderLines);
