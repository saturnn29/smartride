using Microsoft.AspNetCore.Mvc;
using SmartRide.Models;
using SmartRide.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartRide.Controllers
{
    [Route("api/adding-payment-methods")]
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

            var validPaymentTypes = Enum.GetValues(typeof(PaymentType)).Cast<PaymentType>();
            if (!validPaymentTypes.Contains(paymentRequest.PaymentType))
            {
                return BadRequest("Invalid payment type.");
            }

            // Validate required fields based on payment type
            if ((paymentRequest.PaymentType == PaymentType.CREDIT_CARD || paymentRequest.PaymentType == PaymentType.DEBIT_CARD) &&
                (string.IsNullOrEmpty(paymentRequest.CardNumber) ||
                 string.IsNullOrEmpty(paymentRequest.ExpiryDate) ||
                 string.IsNullOrEmpty(paymentRequest.CardHolderName)))
            {
                return BadRequest("Card details are required for credit/debit card payments.");
            }

            if (paymentRequest.PaymentType == PaymentType.PAYPAL && string.IsNullOrEmpty(paymentRequest.PayPalEmail))
            {
                return BadRequest("PayPal email is required for PayPal payments.");
            }

            // For non-PayPal payments, ensure PayPal email is null
            if (paymentRequest.PaymentType != PaymentType.PAYPAL)
            {
                paymentRequest.PayPalEmail = null;
            }

            // For non-card payments, ensure card details are null
            if (paymentRequest.PaymentType != PaymentType.CREDIT_CARD &&
                paymentRequest.PaymentType != PaymentType.DEBIT_CARD)
            {
                paymentRequest.CardNumber = null;
                paymentRequest.ExpiryDate = null;
                paymentRequest.CardHolderName = null;
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