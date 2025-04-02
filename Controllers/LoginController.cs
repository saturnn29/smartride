using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartRide.Models;
using System.Threading.Tasks;

namespace SmartRide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // === User Login ===
        [HttpPost]
        [Route("UserLogin")]
        public async Task<IActionResult> UserLogin([FromBody] UserLogin loginRequest)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("smartride")))
            {
                string query = "SELECT user_id FROM Users WHERE email = @email AND password = @password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@email", loginRequest.email);
                    cmd.Parameters.AddWithValue("@password", loginRequest.password);

                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int userId = reader.GetInt32(0);
                            return Ok(new { user_id = userId, message = "Login successful" });
                        }
                        else
                        {
                            return Unauthorized(new { message = "Invalid email or password" });
                        }
                    }
                }
            }
        }

        [HttpPost]
        [Route("DriverLogin")]
        public IActionResult DriverLogin(DriverLogin loginRequest)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("smartride")))
            {
                con.Open();

                // Check email and get user_id, password
                string userQuery = "SELECT user_id, password FROM Users WHERE email = @email";
                using (SqlCommand userCmd = new SqlCommand(userQuery, con))
                {
                    userCmd.Parameters.AddWithValue("@email", loginRequest.email);

                    using (SqlDataReader reader = userCmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return Unauthorized(new { message = "Invalid email or password." });
                        }

                        int userId = reader.GetInt32(0);
                        string storedPassword = reader.GetString(1);
                        reader.Close();

                        // Compare passwords (if password is encrypted, use BCrypt to check)
                        if (storedPassword != loginRequest.password)
                        {
                            return Unauthorized(new { message = "Invalid email or password." });
                        }

                        // Check if the user is a driver
                        string driverQuery = "SELECT COUNT(*) FROM Drivers WHERE driver_id = @user_id";
                        using (SqlCommand driverCmd = new SqlCommand(driverQuery, con))
                        {
                            driverCmd.Parameters.AddWithValue("@user_id", userId);
                            int isDriver = (int)driverCmd.ExecuteScalar();

                            // Returns user_id and isDriver
                            return Ok(new { user_id = userId, isDriver = (isDriver > 0) });
                        }
                    }
                }
            }
        }


        // === Manager Login ===
        [HttpPost]
        [Route("ManagerLogin")]
        public IActionResult ManagerLogin(ManagersLogin loginRequest)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("smartride")))
            {
                con.Open();

                // Step 1: Check if the email exists in the Users table and fetch the user_id and password
                string userQuery = "SELECT user_id, password FROM Users WHERE email = @email";
                using (SqlCommand userCmd = new SqlCommand(userQuery, con))
                {
                    userCmd.Parameters.AddWithValue("@email", loginRequest.email);

                    using (SqlDataReader reader = userCmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return Unauthorized(new { message = "Invalid email or password." });
                        }

                        int userId = reader.GetInt32(0);
                        string storedPassword = reader.GetString(1);
                        reader.Close();

                        // Compare password (use bcrypt or other hashing methods if needed)
                        if (storedPassword != loginRequest.password)
                        {
                            return Unauthorized(new { message = "Invalid email or password." });
                        }

                        // Step 2: Check if the user is assigned as a manager in the Managers table
                        string managerQuery = "SELECT COUNT(*) FROM Managers WHERE manager_id = @user_id";
                        using (SqlCommand managerCmd = new SqlCommand(managerQuery, con))
                        {
                            managerCmd.Parameters.AddWithValue("@user_id", userId);
                            int managerExists = (int)managerCmd.ExecuteScalar();

                            if (managerExists == 0)
                            {
                                return Unauthorized(new { message = "You are not assigned as a manager." });
                            }
                        }

                        // Return user_id and isManager status
                        return Ok(new { user_id = userId, isManager = true });
                    }
                }
            }
        }

    }
}