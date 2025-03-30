using prueba.Entities;

namespace prueba.Interfaces
{
    public interface ISessionCacheService
    {
        Task<UserSession> GetSessionAsync(string token);
        Task SetSessionAsync(string token, UserSession session);
        Task RemoveSessionAsync(string token);
    }
}