using Microsoft.EntityFrameworkCore;
using QuickDelivery.Core.Entities;
using QuickDelivery.Core.Interfaces.Repositories;
using QuickDelivery.Database;

namespace QuickDelivery.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbContext.Users
                .Include(u => u.Customer)
                .Include(u => u.Partner)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users
                .Include(u => u.Customer)
                .Include(u => u.Partner)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users
                .Include(u => u.Customer)
                .Include(u => u.Partner)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users
                .Include(u => u.Customer)
                .Include(u => u.Partner)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User> CreateAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _dbContext.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}