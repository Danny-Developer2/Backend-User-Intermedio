using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using prueba.Dto;
using prueba.Entities;

namespace prueba.Interfaces
{
    public interface ISessionService
    {
        Task<(bool success, string message)> StoreSessionAsync(ActivateSessionDTO sessionDto);
        Task RemoveUserSessionsAsync(Guid userId);
        Task<bool> ValidateAndUpdateLoginStatus(User user);
        Task<bool> ValidateSessionAsync(Guid userId, string tokenHash);
        
    }
}