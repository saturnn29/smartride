namespace SmartRide.Models;

public class Driver
{
    public int DriverId { get; set; }
    public string LicenseNumber { get; set; }
    public string VehicleDetails { get; set; }
    public decimal DriverRating { get; set; }
    public bool IsAvailable { get; set; }

    public virtual ICollection<Rides> Rides { get; set; } // Navigation property for rides
}
