using Microsoft.AspNetCore.Mvc;
using SmartRide.Services;
using SmartRide.Models;
using System.Threading.Tasks;

namespace SmartRide.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // ✅ Get Saved Payment Methods for a Passenger
        [HttpGet("GetPaymentMethods/{passenger_id}")]
        public async Task<IActionResult> GetPaymentMethods(int passenger_id)
        {
            var methods = await _paymentService.GetPaymentMethodsByPassenger(passenger_id);
            if (methods == null || !methods.Any())
            {
                return NotFound(new { success = false, message = "No saved payment methods found." });
            }
            return Ok(methods);
        }

        // ✅ Process Payment Using a Saved Payment Method
    //    [HttpPost("ProcessPayment")]
    //    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPayment request)
    //    {
    //        if (request == null || request.Amount <= 0)
    //        {
    //            return BadRequest(new { success = false, message = "Invalid request" });
    //        }

    //        // Retrieve saved payment method if PaymentMethodId is provided
    //        if (request.PaymentMethodId.HasValue)
    //        {
    //            var savedMethod = await _paymentService.GetPaymentMethodById(request.PassengerId, request.PaymentMethodId.Value);
    //            if (savedMethod == null)
    //            {
    //                return BadRequest(new { success = false, message = "Saved payment method not found." });
    //            }

    //            request.PaymentType = savedMethod.PaymentType; // Use saved method's type

    //            if (savedMethod.PaymentType == "CREDIT_CARD" || savedMethod.PaymentType == "DEBIT_CARD")
    //            {
    //                var cardRequest = new CardPaymentRequest
    //                {
    //                    Amount = request.Amount,
    //                    PassengerId = request.PassengerId,
    //                    PaymentMethodId = request.PaymentMethodId,
    //                    PaymentType = savedMethod.PaymentType,
    //                    CardNumber = savedMethod.CardNumber,
    //                    ExpiryDate = savedMethod.ExpiryDate,
    //                    CardHolderName = savedMethod.CardHolderName
    //                };

    //                string result = await _paymentService.ProcessCardPayment(
    //                    cardRequest.CardNumber,
    //                    cardRequest.ExpiryDate,
    //                    cardRequest.CardHolderName,
    //                    cardRequest.Amount
    //                );
    //                return Ok(new { success = true, message = result });
    //            }
    //            else if (savedMethod.PaymentType == "PAYPAL")
    //            {
    //                return Ok(new { success = true, message = $"PayPal payment processed for {savedMethod.PayPalEmail}" });
    //            }
    //            else if (savedMethod.PaymentType == "CASH")
    //            {
    //                return Ok(new { success = true, message = "Cash payment pending. Collect payment on delivery." });
    //            }

    //            return BadRequest(new { success = false, message = "Unsupported saved payment type" });
    //        }

    //        return BadRequest(new { success = false, message = "Payment method ID is required" });
    //    }
    }
}