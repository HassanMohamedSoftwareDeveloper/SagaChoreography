namespace InventoryService.Entities;

public class Inventory
{
    public Guid Id { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}
