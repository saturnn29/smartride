using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartRide.Models
{
    [Table("Users")] // Ensure the table name matches
    public class Passengers
    {
        [Key]
        [Column("user_id")] // Update to match the actual column name
        public int Id { get; set; }

        [Column("email")] // Update to match the actual column name
        public string Email { get; set; }

        [Column("username")] // Update to match the actual column name
        public string Name{ get; set; }
    }
}