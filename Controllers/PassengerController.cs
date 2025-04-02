using Microsoft.AspNetCore.Mvc;
using SmartRide.Services;
using SmartRide.Models;
using System.Threading.Tasks;

namespace SmartRide.Controllers
{
    [Route("api/passengers")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private readonly PassengerService _passengerService;

        public PassengerController(PassengerService passengerService)
        {
            _passengerService = passengerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterPassenger([FromBody] PassengerDTO passengerDto)
        {
            if (passengerDto == null || passengerDto.UserId <= 0)
            {
                return BadRequest("Invalid passenger data.");
            }

            var passenger = await _passengerService.RegisterPassenger(passengerDto.UserId);

            return Ok(passenger);
        }
    }
}
