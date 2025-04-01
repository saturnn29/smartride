using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRide.Models
{
    public enum PaymentType
    {
        CREDIT_CARD = 1,
        DEBIT_CARD = 2,
        PAYPAL = 3,
        CASH = 4
    }

    public class PaymentMethods
    {
        [Key]
        [Column("payment_method_id")]
        public int PaymentMethodId { get; set; }

        [Required]
        [Column("payment_type_id")]
        public PaymentType PaymentType { get; set; }  // Changed to enum

        [Column("passenger_id")]
        public int PassengerId { get; set; }

        [Column("card_number")]
        public string? CardNumber { get; set; }

        [Column("expiry_date")]
        public string? ExpiryDate { get; set; }

        [Column("card_holder_name")]
        public string? CardHolderName { get; set; }

        [Column("paypal_email")]
        public string? PayPalEmail { get; set; }
    }
}
