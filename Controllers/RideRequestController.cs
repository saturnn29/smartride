using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartRide.Data;
using SmartRide.Models;
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
                return BadRequest("Invalid request data.");
            }

            // Set initial status and nullify DriverId
            request.Status = "PENDING";
            request.DriverId = null;

            // Save the ride request in the database
            _context.RideRequests.Add(request);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Received ride request: {JsonConvert.SerializeObject(request)}");

            // Notify all connected drivers about the new ride request
            await _hubContext.Clients.All.SendAsync("ReceiveRideRequest", request);

            // Optionally notify the passenger that the request was created
            await _hubContext.Clients.Client(request.PassengerId.ToString()).SendAsync("RideRequestCreated", request);

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

        // Driver accepts a ride request
        [HttpPut("AcceptRideRequest/{driverId}")]
        public async Task<IActionResult> AcceptRideRequest(int driverId, [FromBody] int requestId)
        {
            var rideRequest = await _context.RideRequests.FindAsync(requestId);
            if (rideRequest == null)
            {
                return NotFound("Ride request not found.");
            }

            if (rideRequest.Status != "PENDING")
            {
                return BadRequest("Ride request is not available.");
            }

            // Update the status and assign the driver
            rideRequest.DriverId = driverId;
            rideRequest.Status = "ACCEPTED";

            _context.RideRequests.Update(rideRequest);
            await _context.SaveChangesAsync();

            // Notify the passenger that a driver has accepted the ride
            await _hubContext.Clients.Client(rideRequest.PassengerId.ToString()).SendAsync("RideRequestAccepted", rideRequest);

            return Ok("Ride request accepted.");
        }

        // Cancel a ride request
        [HttpPut("CancelRide/{requestId}")]
        public async Task<IActionResult> CancelRide(int requestId)
        {
            var rideRequest = await _context.RideRequests.FindAsync(requestId);
            if (rideRequest == null)
            {
                return NotFound("Ride request not found.");
            }

            if (rideRequest.Status == "COMPLETED")
            {
                return BadRequest("Cannot cancel a completed ride.");
            }

            // Update status to CANCELLED
            rideRequest.Status = "CANCELLED";
            _context.RideRequests.Update(rideRequest);
            await _context.SaveChangesAsync();

            // Notify all connected clients that the ride was canceled
            await _hubContext.Clients.All.SendAsync("RideRequestCancelled", rideRequest);

            return Ok("Ride canceled successfully.");
        }

        // Get all ride requests (optional, for monitoring or admin purposes)
        [HttpGet("GetAllRideRequests")]
        public async Task<IActionResult> GetAllRideRequests()
        {
            var rideRequests = await _context.RideRequests.ToListAsync();
            return Ok(rideRequests);
        }
    }
}
