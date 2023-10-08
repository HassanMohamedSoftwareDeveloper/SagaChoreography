using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Database;
using PaymentService.Models;

namespace PaymentService.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PaymentController(PaymentDbContext context) : ControllerBase
{
    #region Actions :
    public async Task<IActionResult> GetPayments(CancellationToken cancellationToken = default)
    {
        var payments = await context.Payments
             .AsNoTracking()
             .Select(payment => new PaymentModel(payment.OrderId, payment.User, payment.Amount, payment.PaymentDate))
             .ToListAsync(cancellationToken);

        return Ok(payments);
    }
    #endregion
}
