using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartRide.Models;

namespace SmartRide.Models
{
    public class Passengers
    {
        [Key]
        [Column("passenger_id")]
        public int PassengerId { get; set; }  // This is both the PK and FK to Users table

        // Navigation property to User
        [ForeignKey("PassengerId")]  // This specifies that PassengerId is the FK to User
        public User User { get; set; }

        [ForeignKey("PaymentMethod")]
        [Column("default_method_id")]
        public int? DefaultMethodId { get; set; }

        public PaymentMethods PaymentMethod { get; set; }
    }
}
