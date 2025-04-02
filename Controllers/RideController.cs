using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartRide.Data;
using SmartRide.Models;
using System;
using System.Threading.Tasks;

namespace SmartRide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RideController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<RideRequestHub> _hubContext;

        public RideController(AppDbContext context, IHubContext<RideRequestHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Create a new ride based on RideRequest
        [HttpPost("CreateRide")]
        public async Task<IActionResult> CreateRide([FromBody] RideRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data.");
            }

            // Check if the ride request exists
            var rideRequest = await _context.RideRequests.FindAsync(request.RequestId);
            if (rideRequest == null)
            {
                return NotFound("Ride request not found.");
            }

            // Create a new Ride record (reuse RideRequest properties)
            var ride = new Rides
            {
                RequestId = request.RequestId,
                DriverId = request.DriverId ?? 0, // Assign driver if available
                CurrentLocationLat = (float)request.PickupLat,
                CurrentLocationLong = (float)request.PickupLong,
                ActualPickupTime = DateTime.Now, // Set current time as pickup time
                Status = "IN_PROGRESS", // Default status
                ActualDropoffTime = null // Not set yet
            };

            _context.Rides.Add(ride);
            await _context.SaveChangesAsync();

            // Notify the passenger about the ride creation
            await _hubContext.Clients.Client(request.PassengerId.ToString()).SendAsync("RideCreated", ride);

            return CreatedAtAction(nameof(GetRide), new { id = ride.RideId }, ride);
        }

        // Get ride by ID
        [HttpGet("GetRide/{id}")]
        public async Task<IActionResult> GetRide(int id)
        {
            var ride = await _context.Rides.FindAsync(id);
            if (ride == null)
            {
                return NotFound("Ride not found.");
            }
            return Ok(ride);
        }

        // Update the status and drop-off time for a ride
        [HttpPatch("UpdateRide/{rideId}")]
        public async Task<IActionResult> UpdateRide(int rideId, [FromBody] RideUpdateRequest updateRequest)
        {
            var ride = await _context.Rides.FindAsync(rideId);
            if (ride == null)
            {
                return NotFound("Ride not found.");
            }

            // Update the status and drop-off time
            ride.Status = updateRequest.Status;
            ride.ActualDropoffTime = updateRequest.ActualDropoffTime ?? ride.ActualDropoffTime;

            _context.Rides.Update(ride);
            await _context.SaveChangesAsync();

            // Notify both passenger and driver about the update
            var rideRequest = await _context.RideRequests.FindAsync(ride.RequestId);
            await _hubContext.Clients.Client(rideRequest.PassengerId.ToString()).SendAsync("RideUpdated", ride);
            await _hubContext.Clients.Client(ride.DriverId.ToString()).SendAsync("RideUpdated", ride);

            return Ok(ride);
        }

        // Get all rides (optional for monitoring or admin purposes)
        [HttpGet("GetAllRides")]
        public async Task<IActionResult> GetAllRides()
        {
            var rides = await _context.Rides.ToListAsync();
            return Ok(rides);
        }
    }
}