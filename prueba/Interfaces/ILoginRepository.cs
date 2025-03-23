using System.Threading.Tasks;
using prueba.Dto;
using prueba.Entities;
using Microsoft.AspNetCore.Http;

namespace prueba.Interfaces
{
    public interface ILoginRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> LoginAsync(string username, string password);
        // Task<bool> ValidateAndUpdateLoginStatus(User user);
        // Task<bool> ValidateAccountStatus(User user);
        Task<UserDTO?> CreateUserAsync(User user);  
        Task<string> LogoutAsync(string token);

        Task<User?> ValidateTokenAsync(string token);

    }
}