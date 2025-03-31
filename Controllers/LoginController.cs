using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SmartRide.Models;

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


        [HttpPost]
        [Route("UserLogin")]
        public IActionResult UserLogin(UserLogin loginRequest)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("smartride")))
            {
                string query = "SELECT * FROM Users WHERE username = @username AND password = @password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", loginRequest.username);
                    cmd.Parameters.AddWithValue("@password", loginRequest.password); 

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            return Ok(new { message = "Login successful" });
                        }
                        else
                        {
                            return Unauthorized(new { message = "Invalid username or password" });
                        }
                    }
                }
            }
        }

        [HttpPost]
        [Route("DriverLogin")]
        public IActionResult driverlogin(DriverLogin loginRequest)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("smartride")))
            {
                con.Open();

                // Check if username and password are correct in Users table
                string userQuery = "SELECT user_id, password FROM Users WHERE username = @username";

                using (SqlCommand userCmd = new SqlCommand(userQuery, con))
                {
                    userCmd.Parameters.AddWithValue("@username", loginRequest.username);
                    using (SqlDataReader reader = userCmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return Unauthorized("Invalid username or password.");
                        }

                        int userId = reader.GetInt32(0);
                        string storedPassword = reader.GetString(1);

                        if (storedPassword != loginRequest.password)
                        {
                            return Unauthorized("Invalid username or password.");
                        }

                        reader.Close();

                        // Check if user is a registered driver with the provided license number
                        string driverQuery = "SELECT COUNT(*) FROM Drivers WHERE driver_id = @user_id AND license_number = @license_number";
                        using (SqlCommand driverCmd = new SqlCommand(driverQuery, con))
                        {
                            driverCmd.Parameters.AddWithValue("@user_id", userId);
                            driverCmd.Parameters.AddWithValue("@license_number", loginRequest.license_number);
                            int driverExists = (int)driverCmd.ExecuteScalar();

                            if (driverExists == 0)
                            {
                                return Unauthorized("Driver account not found. Check license number.");
                            }
                        }
                    }
                }

                return Ok("Driver login successful.");
            }
        }

        [HttpPost]
        [Route("ManagersLogin")]
        public IActionResult managerlogin(ManagersLogin loginRequest)
        {
            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("smartride")))
            {
                con.Open();

                // Check if username and password are correct in Users table
                string userQuery = "SELECT user_id, password FROM Users WHERE username = @username";
                using (SqlCommand userCmd = new SqlCommand(userQuery, con))
                {
                    userCmd.Parameters.AddWithValue("@username", loginRequest.username);
                    using (SqlDataReader reader = userCmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return Unauthorized("Invalid username or password.");
                        }

                        int userId = reader.GetInt32(0);
                        string storedPassword = reader.GetString(1);

                        if (storedPassword != loginRequest.password)
                        {
                            return Unauthorized("Invalid username or password.");
                        }

                        reader.Close();

                        // Check if user is a registered manager
                        string managerQuery = "SELECT COUNT(*) FROM Managers WHERE manager_id = @user_id";
                        using (SqlCommand managerCmd = new SqlCommand(managerQuery, con))
                        {
                            managerCmd.Parameters.AddWithValue("@user_id", userId);
                            int managerExists = (int)managerCmd.ExecuteScalar();

                            if (managerExists == 0)
                            {
                                return Unauthorized("You are not assigned as a manager.");
                            }
                        }
                    }
                }

                return Ok("Manager login successful.");
            }
        }

    }
}
