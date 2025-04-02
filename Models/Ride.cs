using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRide.Models
{
    public class Rides
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ride_id")]
        public int RideId { get; set; }

        [Column("request_id")]
        public int RequestId { get; set; } // Foreign key to RideRequest

        [Column("driver_id")]
        public int DriverId { get; set; } // Foreign key to Driver

        [Column("current_location_lat")]
        public double CurrentLocationLat { get; set; }

        [Column("current_location_long")]
        public double CurrentLocationLong { get; set; }

        [Column("actual_pickup_time")]
        public DateTime ActualPickupTime { get; set; }

        [Column("actual_dropoff_time")]
        public DateTime? ActualDropoffTime { get; set; } // Nullable for unfinished rides

        [Column("status")]
        public string Status { get; set; } // "IN_PROGRESS", "COMPLETED", etc.
    }
}