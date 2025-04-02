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
    public class RideRequestController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<RideRequestHub> _hubContext;

        public RideRequestController(AppDbContext context, IHubContext<RideRequestHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // Create a new ride request
        [HttpPost("CreateRideRequest")]
        public async Task<IActionResult> CreateRideRequest([FromBody] RideRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid ride request data.");
            }

            _context.RideRequests.Add(request);
            await _context.SaveChangesAsync();

            // Notify all connected clients about the new ride request
            await _hubContext.Clients.All.SendAsync("NewRideRequest", request);

            return CreatedAtAction(nameof(GetRideRequest), new { id = request.RequestId }, request);
        }

        // Get ride request by ID
        [HttpGet("GetRideRequest/{id}")]
        public async Task<IActionResult> GetRideRequest(int id)
        {
            var rideRequest = await _context.RideRequests.FindAsync(id);
            if (rideRequest == null)
            {
                return NotFound("Ride request not found.");
            }
            return Ok(rideRequest);
        }

        // Update ride request status
        [HttpPatch("UpdateRideRequest/{id}")]
        public async Task<IActionResult> UpdateRideRequest(int id, [FromBody] RideUpdateRequest updateRequest)
        {
            var rideRequest = await _context.RideRequests.FindAsync(id);
            if (rideRequest == null)
            {
                return NotFound("Ride request not found.");
            }

            // Update the status of the ride request
            rideRequest.Status = updateRequest.Status;

            // Update driver ID if provided, regardless of status
            if (updateRequest.DriverId != null)
            {
                rideRequest.DriverId = updateRequest.DriverId;
            }

            _context.RideRequests.Update(rideRequest);
            await _context.SaveChangesAsync();

            // If the ride request is accepted, create a new ride entry
            if (updateRequest.Status == "ACCEPTED")
            {
                if (updateRequest.DriverId == null)
                {
                    return BadRequest("A driver must be assigned when accepting a ride request.");
                }

                var ride = new Rides
                {
                    RequestId = rideRequest.RequestId,
                    DriverId = updateRequest.DriverId.Value,
                    CurrentLocationLat = rideRequest.PickupLat,
                    CurrentLocationLong = rideRequest.PickupLong,
                    ActualPickupTime = DateTime.Now,
                    Status = "IN_PROGRESS",
                    ActualDropoffTime = null
                };

                _context.Rides.Add(ride);
                await _context.SaveChangesAsync();

                // Notify the passenger and driver about the ride creation
                await _hubContext.Clients.Client(rideRequest.PassengerId.ToString()).SendAsync("RideCreated", ride);
                await _hubContext.Clients.Client(updateRequest.DriverId.ToString()).SendAsync("RideCreated", ride);
            }

            // Notify the passenger about the update
            await _hubContext.Clients.Client(rideRequest.PassengerId.ToString()).SendAsync("RideRequestUpdated", rideRequest);

            return Ok(rideRequest);
        }


        // Get all ride requests (optional for monitoring or admin purposes)
        [HttpGet("GetAllRideRequests")]
        public async Task<IActionResult> GetAllRideRequests()
        {
            var rideRequests = await _context.RideRequests.ToListAsync();
            return Ok(rideRequests);
        }
    }
}