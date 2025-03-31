using Microsoft.AspNetCore.Mvc;
using SmartRide.Models;
using SmartRide.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartRide.Controllers
{
    [Route("api/payment-methods")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentMethodsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPaymentMethod([FromBody] PaymentMethods paymentRequest)
        {
            if (paymentRequest == null)
            {
                return BadRequest("Invalid request data.");
            }

            var validPaymentTypes = new[] { "CREDIT_CARD", "DEBIT_CARD", "PAYPAL", "CASH" };
            if (!validPaymentTypes.Contains(paymentRequest.PaymentType))
            {
                return BadRequest("Invalid payment type.");
            }

            // Validate required fields based on payment type
            if ((paymentRequest.PaymentType == "CREDIT_CARD" || paymentRequest.PaymentType == "DEBIT_CARD") &&
                (string.IsNullOrEmpty(paymentRequest.CardNumber) ||
                 string.IsNullOrEmpty(paymentRequest.ExpiryDate) ||
                 string.IsNullOrEmpty(paymentRequest.CardHolderName)))
            {
                return BadRequest("Card details are required for credit/debit card payments.");
            }

            if (paymentRequest.PaymentType == "PAYPAL" && string.IsNullOrEmpty(paymentRequest.PayPalEmail))
            {
                return BadRequest("PayPal email is required for PayPal payments.");
            }

            _context.PaymentMethods.Add(paymentRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPaymentMethod), new { id = paymentRequest.PaymentMethodId }, paymentRequest);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentMethod(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null)
            {
                return NotFound();
            }
            return Ok(paymentMethod);
        }
    }
}