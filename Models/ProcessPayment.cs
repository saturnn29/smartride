namespace SmartRide.Models
{
    // General Payment Request (Used for all payment types)
    public class ProcessPayment
    {
        public decimal Amount { get; set; }
        public int PassengerId { get; set; }  // Identifies the user making the payment
        public int? PaymentMethodId { get; set; }  // If null, a new method must be added
        public string PaymentType { get; set; } // ('CREDIT_CARD', 'DEBIT_CARD', 'PAYPAL', 'CASH')
    }

    // Card Payment Request (When adding a new credit/debit card)
    public class CardPaymentRequest : ProcessPayment
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CardHolderName { get; set; }
    }

    // PayPal Payment Request (When adding a new PayPal account)
    public class PayPalPaymentRequest : ProcessPayment
    {
        public string PayPalEmail { get; set; }
    }
}