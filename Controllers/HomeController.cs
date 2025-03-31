using Microsoft.AspNetCore.Mvc;

namespace SmartRide.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Message = "Testing.." });
        }
    }
}
