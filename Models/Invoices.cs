using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRide.Models
{
    public class Invoices
    {
        [Key]
        [Column("invoice_id")]
        public int InvoiceId { get; set; }

        [Required]
        [Column("ride_id")]
        public int RideId { get; set; }

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("payment_method_id")]
        public int? PaymentMethodId { get; set; }

        [Required]
        [Column("payment_status")]
        public string PaymentStatus { get; set; }

        [Required]
        [Column("issued_at")]
        public DateTime IssuedAt { get; set; }

        [ForeignKey("PaymentMethodId")]
        public PaymentMethods PaymentMethod { get; set; }

        //[ForeignKey("RideId")]
        //public Rides Ride { get; set; }
    }
}
