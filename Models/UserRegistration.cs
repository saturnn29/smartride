using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; 

namespace SmartRide.Models
{
    public class UserRegistration
    {
        [Required]
        [StringLength(50)]
        public string username { get; set; }

        [Required]
        [StringLength(255)]
        public string password { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string email { get; set; }
    }
}
