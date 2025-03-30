using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace prueba.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly int _maxRequests = 100;
        private readonly int _timeWindow = 60; // segundos

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var cacheKey = $"RateLimit_{ipAddress}";
            var requests = _cache.GetOrCreate(cacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_timeWindow);
                return 0;
            });

            if (requests >= _maxRequests)
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsJsonAsync(new { message = "Too many requests" });
                return;
            }

            _cache.Set(cacheKey, requests + 1, TimeSpan.FromSeconds(_timeWindow));
            await _next(context);
        }
    }
}