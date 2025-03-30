using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using prueba.Entities;
using prueba.Interfaces;

namespace prueba.Services
{
    public class SessionCacheService : ISessionCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public SessionCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<UserSession> GetSessionAsync(string token)
        {
            var cacheKey = $"Session_{token}";
            return await Task.FromResult(_cache.Get<UserSession>(cacheKey)!);
        }

        public async Task SetSessionAsync(string token, UserSession session)
        {
            var cacheKey = $"Session_{token}";
            _cache.Set(cacheKey, session, _cacheDuration);
            await Task.CompletedTask;
        }

        public async Task RemoveSessionAsync(string token)
        {
            var cacheKey = $"Session_{token}";
            _cache.Remove(cacheKey);
            await Task.CompletedTask;
        }
    }
}