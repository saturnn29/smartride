namespace SmartRide.Models
{
    public class RideUpdateRequest
    {
        public string Status { get; set; }
        public DateTime? ActualDropoffTime { get; set; }
    }
}
