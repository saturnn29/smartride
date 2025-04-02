using Microsoft.AspNetCore.Mvc;
using SmartRide.Models;
using SmartRide.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace SmartRide.Controllers
{
    [Route("api/AddingPaymentMethods")]
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

            // Ensure the payment type is a valid enum value
            if (!Enum.IsDefined(typeof(PaymentType), paymentRequest.PaymentType))
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

            if (paymentRequest.PaymentType != PaymentType.CREDIT_CARD &&
                paymentRequest.PaymentType != PaymentType.DEBIT_CARD)
            {
                paymentRequest.CardNumber = null;
                paymentRequest.ExpiryDate = null;
                paymentRequest.CardHolderName = null;
            }

            // Check for duplicate card information
            if (paymentRequest.PaymentType == PaymentType.CREDIT_CARD ||
                paymentRequest.PaymentType == PaymentType.DEBIT_CARD)
            {
                var duplicateCard = await _context.PaymentMethods
                    .Where(pm => pm.CardNumber == paymentRequest.CardNumber &&
                                 pm.PassengerId == paymentRequest.PassengerId)
                    .FirstOrDefaultAsync();

                if (duplicateCard != null)
                {
                    return BadRequest("This card is already registered for this passenger.");
                }
            }

            _context.PaymentMethods.Add(paymentRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPaymentMethodsByPassenger), new { id = paymentRequest.PaymentMethodId }, paymentRequest);
        }

        [HttpGet("passenger/{passengerId}")]
        public async Task<IActionResult> GetPaymentMethodsByPassenger(int passengerId)
        {
            var paymentMethods = await _context.PaymentMethods
                .Where(pm => pm.PassengerId == passengerId)
                .ToListAsync();

            if (paymentMethods == null || !paymentMethods.Any())
            {
                return NotFound("No payment methods found for the given passenger.");
            }

            return Ok(paymentMethods);
        }
    }
}