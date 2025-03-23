using System.Collections.Generic;
using System.Threading.Tasks;
using prueba.Entities;

namespace prueba.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(Guid id);
        Task<bool> UserExistsAsync(string email);
        Task<bool> SaveChangesAsync();
    }
}