using Microsoft.AspNetCore.SignalR;
using SmartRide.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class RideRequestHub : Hub
{
    private static ConcurrentDictionary<int, RideRequest> ActiveRequests = new();
    private static ConcurrentDictionary<string, int> DriverAssignments = new();
    private static ConcurrentDictionary<int, string> PassengerConnections = new();

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task RequestRide(int passengerId, double pickupLat, double pickupLong, double dropoffLat, double dropoffLong, float fareAmount)
    {
        // Store passenger connection
        PassengerConnections[passengerId] = Context.ConnectionId;

        var rideRequest = new RideRequest
        {
            PassengerId = passengerId,
            PickupLat = pickupLat,
            PickupLong = pickupLong,
            DropoffLat = dropoffLat,
            DropoffLong = dropoffLong,
            FareAmount = fareAmount,
            Status = "PENDING",
            RequestedAt = DateTime.Now
        };

        ActiveRequests[rideRequest.RequestId] = rideRequest;

        // Notify passenger
        await Clients.Caller.SendAsync("NewRideRequest", rideRequest);

        // Notify drivers
        await Clients.Others.SendAsync("NewRideRequestForOthers", rideRequest);

        // Log to track if it's being sent to the drivers
        Console.WriteLine($"New ride request sent to all connected drivers: {rideRequest.RequestId}");
    }

    public async Task AcceptRide(int driverId, int requestId)
    {
        if (ActiveRequests.TryGetValue(requestId, out var rideRequest) && rideRequest.Status == "PENDING")
        {
            rideRequest.Status = "ACCEPTED";
            rideRequest.DriverId = driverId;
            DriverAssignments[Context.ConnectionId] = requestId;

            // Notify driver
            await Clients.Caller.SendAsync("RideRequestAccepted", rideRequest);

            // Notify passenger using stored connection
            if (PassengerConnections.TryGetValue(rideRequest.PassengerId, out string passengerConnection))
            {
                await Clients.Client(passengerConnection).SendAsync("RideRequestAcceptedByDriver", rideRequest);
            }
        }
        else
        {
            await Clients.Caller.SendAsync("Error", "Ride request is not available or already accepted.");
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (DriverAssignments.TryRemove(Context.ConnectionId, out int requestId) && ActiveRequests.ContainsKey(requestId))
        {
            ActiveRequests[requestId].Status = "DRIVER_DISCONNECTED";
            await Clients.All.SendAsync("RideRequestDriverDisconnected", requestId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}