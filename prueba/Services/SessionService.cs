using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using prueba.Data;
using prueba.Dto;
using prueba.Entities;
using prueba.Interfaces;

namespace prueba.Services
{
    public class SessionService : ISessionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SessionService> _logger;

        public SessionService(AppDbContext context, ILogger<SessionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(bool success, string message)> StoreSessionAsync(ActivateSessionDTO sessionDto)
        {
            // Check for existing active session
            var existingSession = await _context.UserSessions
                .FirstOrDefaultAsync(s =>
                    s.UserId == sessionDto.UserId &&
                    s.ExpiresAt > DateTime.UtcNow);

            if (existingSession != null)
            {
                return (false, "User already has an active session");
            }

            var session = new UserSession
            {
                UserId = sessionDto.UserId,
                TokenHash = sessionDto.TokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = sessionDto.Expiration
            };

            await _context.UserSessions.AddAsync(session);
            await _context.SaveChangesAsync();

            return (true, "Session created successfully");
        }

        public async Task RemoveUserSessionsAsync(Guid userId)
        {
            var userSessions = await _context.UserSessions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            if (userSessions.Any())
            {
                _context.UserSessions.RemoveRange(userSessions);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateAndUpdateLoginStatus(User user)
        {
            try
            {
                if (user == null || string.IsNullOrEmpty(user.Email)) return false;

                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating login status: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> ValidateSessionAsync(Guid userId, string tokenHash)
        {
            try
            {
                var session = await _context.UserSessions
                    .FirstOrDefaultAsync(s =>
                        s.UserId == userId &&
                        s.TokenHash == tokenHash &&
                        s.ExpiresAt > DateTime.UtcNow);

                return session != null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating session: {ex.Message}");
                return false;
            }
        }
    }
}