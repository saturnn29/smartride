using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRide.Models
{
    public class Driver
    {
        [Column("driver_id")]
        public int DriverId { get; set; }

        [Column("license_number")]
        public string LicenseNumber { get; set; }

        [Column("vehicle_details")]
        public string VehicleDetails { get; set; }

        [Column("driver_rating")]
        public decimal DriverRating { get; set; }

        [Column("isAvailable")]
        public bool IsAvailable { get; set; }

        public virtual ICollection<Rides> Rides { get; set; }
    }
}