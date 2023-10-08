namespace PaymentService.Models;

public record PaymentModel(Guid OrderId,
                           string User,
                           decimal Amount,
                           DateTime PaymentDate);
