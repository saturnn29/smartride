using Microsoft.AspNetCore.Mvc;
using SmartRide.Services;
using SmartRide.Models;
using System.Threading.Tasks;
using System;

namespace SmartRide.Controllers
{
    public partial class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly NotificationService _notificationService;

        public PaymentController(
            PaymentService paymentService,
            NotificationService notificationService)
        {
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        // Update ProcessPayment method to include saving to Invoices table
        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPayment request)
        {
            // Use a temporary or mock ride ID for testing purposes
            int rideId = 4; // or generate a mock ride ID dynamically if needed

            if (request == null || request.Amount <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid request" });
            }

            if (request.PaymentMethodId.HasValue)
            {
                var savedMethod = await _paymentService.GetPaymentMethodById(request.PassengerId, request.PaymentMethodId.Value);
                if (savedMethod == null)
                {
                    return BadRequest(new { success = false, message = "Saved payment method not found." });
                }

                var passenger = await _paymentService.GetPassengerById(request.PassengerId);
                if (passenger == null || string.IsNullOrEmpty(passenger.User.Email))
                {
                    return BadRequest(new { success = false, message = "Passenger information not found or email missing." });
                }

                request.PaymentType = savedMethod.PaymentType.ToString();
                bool success = false;
                string message = "";

                try
                {
                    if (savedMethod.PaymentType == PaymentType.CREDIT_CARD || savedMethod.PaymentType == PaymentType.DEBIT_CARD)
                    {
                        var cardRequest = new CardPaymentRequest
                        {
                            Amount = request.Amount,
                            PassengerId = request.PassengerId,
                            PaymentMethodId = request.PaymentMethodId,
                            PaymentType = savedMethod.PaymentType.ToString(),
                            CardNumber = savedMethod.CardNumber,
                            ExpiryDate = savedMethod.ExpiryDate,
                            CardHolderName = savedMethod.CardHolderName
                        };

                        message = await _paymentService.ProcessCardPayment(
                            cardRequest.CardNumber,
                            cardRequest.ExpiryDate,
                            cardRequest.CardHolderName,
                            cardRequest.Amount
                        );
                        success = true;
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Unsupported saved payment type" });
                    }

                    // Save invoice if payment is successful
                    if (success)
                    {
                        // Use the mock ride ID for testing
                        await _paymentService.SaveInvoice(
                            rideId: rideId, // Temporary/mock ride ID
                            amount: request.Amount,
                            paymentMethodId: request.PaymentMethodId.Value,
                            paymentStatus: "PAID"
                        );
                    }
                }
                catch (Exception ex)
                {
                    message = $"Payment failed: {ex.Message}";
                    success = false;
                }

                //_ = _notificationService.SendPaymentEmailNotificationAsync(
                //    request.PassengerId,
                //    passenger.Email,
                //    request.Amount,
                //    request.PaymentType,
                //    success,
                //    message
                //);

                return success ? Ok(new { success = true, message }) : BadRequest(new { success = false, message });
            }

            return BadRequest(new { success = false, message = "Payment method ID is required" });
        }
    }
}
