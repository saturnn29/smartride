using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartRide.Models;

public class RideRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("request_id")]
    public int RequestId { get; set; }

    [Column("passenger_id")]
    public int PassengerId { get; set; }

    [Column("driver_id")]
    public int? DriverId { get; set; }

    [Column("pickup_location_lat")]
    public double PickupLat { get; set; }

    [Column("pickup_location_long")]
    public double PickupLong { get; set; }

    [Column("dropoff_location_lat")]
    public double DropoffLat { get; set; }

    [Column("dropoff_location_long")]
    public double DropoffLong { get; set; }

    [Column("fare_amount")]
    public double FareAmount { get; set; }

    [Column("requested_at")]
    public DateTime RequestedAt { get; set; }

    [Column("status")]
    public string Status { get; set; }

    //[Column("cancel_reason")]
    //public string? CancelReason { get; set; }
}