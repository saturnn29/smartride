using System.ComponentModel.DataAnnotations;

namespace SmartRide.Models
{
    public class PaymentMethods
    {
        [Key]
        public int PaymentMethodId { get; set; }  

        [Required]
        public string PaymentType { get; set; }
        public int PassengerId { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CardHolderName { get; set; }
        public string? PayPalEmail { get; set; }  // Added to match expected property
    }
}
