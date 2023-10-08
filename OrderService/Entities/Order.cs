namespace OrderService.Entities;
public class Order
{
    public Guid Id { get; set; }
    public int Status { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Customer { get; set; } = string.Empty;
    public virtual ICollection<OrderLine> OrderLines { get; set; } = new HashSet<OrderLine>();
}
