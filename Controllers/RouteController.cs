using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace SmartRide.Controllers
{
    [Route("api/map")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly string? _subscriptionKey;

        public MapController(IConfiguration configuration)
        {
            _subscriptionKey = configuration["AzureMaps:SubscriptionKey"];
        }

        [HttpGet("key")]
        public IActionResult GetSubscriptionKey()
        {
            return Ok(new { subscriptionKey = _subscriptionKey });
        }
    }
}