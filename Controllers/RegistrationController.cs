using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartRide.Models;

namespace SmartRide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegistrationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("UserRegistration")]
        public IActionResult userregistration(UserRegistration registration)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("smartride")))
            {
                string query = "INSERT INTO Users (username, password, email, created_at) VALUES (@username, @password, @email, @created_at)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", registration.username);
                    cmd.Parameters.AddWithValue("@password", registration.password); 
                    cmd.Parameters.AddWithValue("@email", registration.email);
                    cmd.Parameters.AddWithValue("@created_at", DateTime.UtcNow); 

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        return Ok("User registered successfully");
                    else
                        return BadRequest("Failed to register user");
                }
            }
        }

        [HttpPost]
        [Route("DriverRegistration")]
        public IActionResult driverregistration(DriverRegistration driver)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("smartride")))
            {
                con.Open();

                // Verify if the user exists
                string getUserQuery = "SELECT user_id FROM Users WHERE email = @email";
                using (SqlCommand getUserCmd = new SqlCommand(getUserQuery, con))
                {
                    getUserCmd.Parameters.AddWithValue("@email", driver.email);
                    object result = getUserCmd.ExecuteScalar();

                    if (result == null)
                    {
                        return BadRequest("User does not exist. Register as a user first.");
                    }

                    int userId = (int)result; // Use user_id as driver_id
                    driver.driver_id = userId;
                }

                // Check if user is already a driver
                string checkDriverQuery = "SELECT COUNT(*) FROM Drivers WHERE driver_id = @driver_id";
                using (SqlCommand checkCmd = new SqlCommand(checkDriverQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@driver_id", driver.driver_id);
                    int driverExists = (int)checkCmd.ExecuteScalar();

                    if (driverExists > 0)
                    {
                        return BadRequest("User is already registered as a driver.");
                    }
                }

                // Insert into Drivers table
                string insertQuery = "INSERT INTO Drivers (driver_id, license_number, vehicle_details, active_status) " +
                                     "VALUES (@driver_id, @license_number, @vehicle_details, @active_status)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.AddWithValue("@driver_id", driver.driver_id);
                    cmd.Parameters.AddWithValue("@license_number", driver.license_number);
                    cmd.Parameters.AddWithValue("@vehicle_details", driver.vehicle_details);
                    cmd.Parameters.AddWithValue("@active_status", driver.active_status);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        return Ok("Driver registered successfully.");
                    else
                        return BadRequest("Failed to register driver.");
                }
            }
        }

    }
}
