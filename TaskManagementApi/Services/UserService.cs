using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Interfaces;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async System.Threading.Tasks.Task<User?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
            return user;
        }

        public async System.Threading.Tasks.Task<bool> VerifyPasswordAsync(User user, string password)
        {
            if (user == null) return false;
            return await System.Threading.Tasks.Task.Run(() =>
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                return result != PasswordVerificationResult.Failed;
            });
        }

        public async System.Threading.Tasks.Task AddUserAsync(User user, string password)
        {
            if (await GetUserByUsernameAsync(user.Username) != null)
            {
                throw new InvalidOperationException($"User with username '{user.Username}' already exists.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
