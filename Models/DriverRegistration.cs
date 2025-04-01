namespace SmartRide.Models
{
    public class DriverRegistration
    {
        public string email { get; set; } // Used for user verification
        public string license_number { get; set; }
        public string vehicle_details { get; set; }
        public bool isAvailable { get; set; } = true; 
        public int driver_id { get; set; } // Internal use, not required in request
    }
}
