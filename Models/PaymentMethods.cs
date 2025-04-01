using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SmartRide.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentType
    {
        CREDIT_CARD,
        DEBIT_CARD,
        CASH
    }

    public class PaymentMethods
    {
        [Key]
        [Column("payment_method_id")]
        public int PaymentMethodId { get; set; }

        [Required]
        [Column("payment_type")]
        [EnumDataType(typeof(PaymentType))]
        public PaymentType PaymentType { get; set; }

        [Column("passenger_id")]
        public int PassengerId { get; set; }

        [Column("card_number")]
        public string? CardNumber { get; set; }

        [Column("expiry_date")]
        public string? ExpiryDate { get; set; }

        [Column("card_holder_name")]
        public string? CardHolderName { get; set; }
    }
}
