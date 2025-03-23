using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using prueba.Entities;

namespace prueba.Interfaces
{
    public interface IAuthenticationService
    {
        string GenerateJwtToken(User user);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
        byte[] CreatePasswordHash(string password, byte[] salt = null!);
        ClaimsPrincipal DecodeJwtToken(string token);
        string HashToken(string token);
    }
}