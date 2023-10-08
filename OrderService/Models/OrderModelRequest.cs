namespace OrderService.Models;

public record OrderModelRequest(string Customer, List<OrderLineModel> OrderLines);
