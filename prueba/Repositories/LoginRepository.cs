using prueba.Interfaces;
using prueba.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;
using prueba.Data;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using prueba.Dto;

namespace prueba.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly ILogger<LoginRepository> _logger;
        private readonly IAuthenticationService _authService;
        private readonly ISessionService _sessionService;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public LoginRepository(
            ILogger<LoginRepository> logger,
            IAuthenticationService authService,
            ISessionService sessionService,
            AppDbContext context,
            IMapper mapper)
        {
            _logger = logger;
            _authService = authService;
            _sessionService = sessionService;
            _context = context;
            _mapper = mapper;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == username);
                _logger.LogInformation($"GetUserByUsername attempt for: {username}, Found: {user != null}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user by username: {ex.Message}");
                return null;
            }
        }


        public async Task<User> LoginAsync(string username, string password)
        {
            try
            {
                var user = await GetUserByUsernameAsync(username);

                if (user == null)
                {
                    _logger.LogWarning($"User not found: {username}");
                    throw new KeyNotFoundException($"User with email {username} not found");
                }

                if (!_authService.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    _logger.LogWarning($"Invalid password for user: {username}");
                    throw new UnauthorizedAccessException("Invalid password");
                }

                bool loginSuccess = await _sessionService.ValidateAndUpdateLoginStatus(user);
                if (!loginSuccess)
                {
                    _logger.LogWarning($"Account validation failed for user: {username}");
                    throw new UnauthorizedAccessException("Account validation failed");
                }

                user.Token = _authService.GenerateJwtToken(user);

                var sessionDto = new ActivateSessionDTO
                {
                    UserId = user.Id,
                    TokenHash = _authService.HashToken(user.Token),
                    Expiration = DateTime.UtcNow.AddDays(7)
                };

                var sessionResult = await _sessionService.StoreSessionAsync(sessionDto);
                if (!sessionResult.success)
                {
                    _logger.LogWarning($"Session validation failed: {sessionResult.message}");
                    throw new UnauthorizedAccessException(sessionResult.message);
                }

                _logger.LogInformation($"Login successful for user: {username}");

                return _mapper.Map<User>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                throw;
            }
        }

        public async Task<string> LogoutAsync(string token)
        {
            try
            {
                var claims = _authService.DecodeJwtToken(token);
                var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    throw new UnauthorizedAccessException("Invalid token");
                }

                var userGuid = Guid.Parse(userId);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userGuid);

                if (user == null || string.IsNullOrEmpty(user.Token))
                {
                    throw new KeyNotFoundException("Invalid token or user not found");
                }

                await _sessionService.RemoveUserSessionsAsync(userGuid);

                user.Token = string.Empty;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Logout successful for user: {user.Email}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Logout error: {ex.Message}");
                throw;
            }
        }

        public async Task<UserDTO?> CreateUserAsync(User user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                {
                    throw new ArgumentException("Email and password are required");
                }

                using var hmac = new HMACSHA512();
                user.PasswordSalt = hmac.Key;
                user.PasswordHash = _authService.CreatePasswordHash(user.Password, user.PasswordSalt);

                user.Password = string.Empty;
                user.Id = Guid.NewGuid();
                user.Token = _authService.GenerateJwtToken(user);
                user.LastLogin = DateTime.UtcNow;

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                var sessionDto = new ActivateSessionDTO
                {
                    UserId = user.Id,
                    TokenHash = _authService.HashToken(user.Token),
                    Expiration = DateTime.UtcNow.AddDays(7)
                };

                await _sessionService.StoreSessionAsync(sessionDto);

                _logger.LogInformation($"User created successfully with ID: {user.Id}");

                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"User creation error: {ex.Message}");
                throw;
            }
        }


        // ... existing code ...

        public async Task<User?> ValidateTokenAsync(string token)
        {
            try
            {
                var claims = _authService.DecodeJwtToken(token);
                var userId = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    _logger.LogWarning("Invalid token: no user ID found in claims");
                    return null;
                }

                var userGuid = Guid.Parse(userId);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userGuid);

                if (user == null)
                {
                    _logger.LogWarning($"User not found for ID: {userId}");
                    return null;
                }

                var isValidSession = await _sessionService.ValidateSessionAsync(userGuid, _authService.HashToken(token));
                if (!isValidSession)
                {
                    _logger.LogWarning($"Invalid session for user: {user.Email}");
                    return null;
                }

                _logger.LogInformation($"Token validated successfully for user: {user.Email}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Token validation error: {ex.Message}");
                return null;
            }
        }

        // ... rest of the class
    }
}