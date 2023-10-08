namespace PaymentService.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public string User { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
}
