using prueba.Interfaces;
using prueba.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prueba.Data;

namespace prueba.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user == null) return false;
            
            _context.Users.Remove(user);
            return await SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}