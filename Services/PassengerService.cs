using SmartRide.Models;
using SmartRide.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartRide.Services
{
    public class PassengerService
    {
        private readonly AppDbContext _dbContext;

        public PassengerService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Passengers> RegisterPassenger(int userId)
        {
            // Check if the user already has a passenger profile - include User data
            var existingPassenger = await _dbContext.Passengers
                .Include(p => p.User)  // This eagerly loads the related User entity
                .FirstOrDefaultAsync(p => p.PassengerId == userId);

            if (existingPassenger != null)
            {
                return existingPassenger; // Return existing profile with User data
            }

            // Check if user exists
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User does not exist");
            }

            // Create new passenger profile
            var newPassenger = new Passengers
            {
                PassengerId = userId
            };

            _dbContext.Passengers.Add(newPassenger);
            await _dbContext.SaveChangesAsync();

            // Reload the passenger with the User relationship
            return await _dbContext.Passengers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PassengerId == userId);
        }
    }
}
