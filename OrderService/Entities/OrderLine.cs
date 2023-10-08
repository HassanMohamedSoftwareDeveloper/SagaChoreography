namespace OrderService.Entities;

public class OrderLine
{
    public Guid Id { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = new();
}
