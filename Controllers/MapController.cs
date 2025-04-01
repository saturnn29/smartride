// RouteController.cs - Handles route calculations using Azure Maps API
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartRide.Controllers
{
    [Route("api/route")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string? _subscriptionKey;

        public RouteController(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _subscriptionKey = configuration["AzureMaps:SubscriptionKey"];
        }

        [HttpGet]
        public async Task<IActionResult> GetRoute(double startLat, double startLon, double endLat, double endLon)
        {
            string routeUrl = $"https://atlas.microsoft.com/route/directions/json?api-version=1.0&query={startLat},{startLon}:{endLat},{endLon}&subscription-key={_subscriptionKey}";

            var response = await _httpClient.GetAsync(routeUrl);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error fetching route data");
            }

            var data = await response.Content.ReadAsStringAsync();
            return Ok(data);
        }
    }
}