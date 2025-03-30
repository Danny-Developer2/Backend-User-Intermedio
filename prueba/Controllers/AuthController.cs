using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using prueba.Dto;
using prueba.Entities;
using prueba.Error;
using prueba.Helpers;
using prueba.Interfaces;

namespace prueba.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ApiControllBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string? _jwtSecretKey;
        private readonly ILogger<AuthController> _logger;
        private readonly IEncryptionService _encryptionService;
        private readonly ISessionCacheService _sessionCache;

        public AuthController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AuthController> logger,
            IConfiguration configuration,
            IEncryptionService encryptionService,
            ISessionCacheService sessionCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _jwtSecretKey = configuration["JwtSettings:SecretKey"];
            _logger = logger;
            _encryptionService = encryptionService;
            _sessionCache = sessionCache;
        }

        private string TokenEncrypt(string token)
        {
            return _encryptionService.Encrypt(token, _jwtSecretKey!);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var user = await _unitOfWork.LoginRepository.LoginAsync(loginDTO.Username, loginDTO.Password);

                var Token = TokenEncrypt(user.Token);

                await _sessionCache.SetSessionAsync(Token, new UserSession
                {
                    UserId = user.Id,
                    TokenHash = user.Token,
                    User = user,
                });



                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("Cookie", Token, cookieOptions);

                var userResponse = _mapper.Map<UserDTO>(user);

                return Ok(new ApiResponse(
                    mensaje: "Login successful",
                    exito: true,
                    datos: userResponse
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new ApiResponse(
                    mensaje: ex.Message,
                    exito: false,
                    error: "Authorization Error"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new ApiResponse(
                    mensaje: ex.Message,
                    exito: false,
                    error: "User Not Found"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                return StatusCode(500, new ApiResponse(
                    mensaje: "An unexpected error occurred",
                    exito: false,
                    error: ex.Message
                ));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] UserDTO userDto)
        {
            try
            {
                var firstNameValidation = UserHelper.ValidateFirstName(userDto.FirstName);
                if (!string.IsNullOrEmpty(firstNameValidation))
                {
                    return BadRequest(new ApiResponse(
                        mensaje: firstNameValidation,
                        exito: false,
                        error: "Validation error"
                    ));
                }

                var lastNameValidation = UserHelper.ValidateLastName(userDto.LastName);
                if (!string.IsNullOrEmpty(lastNameValidation))
                {
                    return BadRequest(new ApiResponse(
                        mensaje: lastNameValidation,
                        exito: false,
                        error: "Validation error"
                    ));
                }

                var emailValidation = UserHelper.ValidateEmail(userDto.Email);
                if (!string.IsNullOrEmpty(emailValidation))
                {
                    return BadRequest(new ApiResponse(
                        mensaje: emailValidation,
                        exito: false,
                        error: "Validation error"
                    ));
                }

                var phoneValidation = UserHelper.ValidatePhone(userDto.Phone);
                if (!string.IsNullOrEmpty(phoneValidation))
                {
                    return BadRequest(new ApiResponse(
                        mensaje: phoneValidation,
                        exito: false,
                        error: "Validation error"
                    ));
                }

                var passwordValidation = LoginHelper.IsValidPassword(userDto.Password);
                if (!string.IsNullOrEmpty(passwordValidation))
                {
                    return BadRequest(new ApiResponse(
                        mensaje: passwordValidation,
                        exito: false,
                        error: "Validation error"
                    ));
                }

                if (await _unitOfWork.LoginRepository.GetUserByUsernameAsync(userDto.Email) != null)
                {
                    return BadRequest(new ApiResponse(
                        mensaje: "El usuario ya existe",
                        exito: false,
                        error: "Duplicate user"
                    ));
                }
                // var user = _mapper.Map<User>(User);
                var user = _mapper.Map<User>(userDto);
                var result = await _unitOfWork.LoginRepository.CreateUserAsync(user);
                var Token = TokenEncrypt(user.Token);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("Cookie", Token, cookieOptions);

                _logger.LogInformation($"User registered successfully: {userDto.Email}");

                return Created($"api/auth/{result!.Email}", new ApiResponse(
                    mensaje: "Usuario registrado exitosamente",
                    exito: true,
                    datos: result
                ));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Registration validation error: {ex.Message}");
                return BadRequest(new ApiResponse(
                    mensaje: ex.Message,
                    exito: false,
                    error: "Validation error"
                ));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Database operation failed: {ex.Message}");
                return StatusCode(500, new ApiResponse(
                    mensaje: "Error al guardar la información del usuario",
                    exito: false,
                    error: ex.Message
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error during registration: {ex.Message}");
                return StatusCode(500, new ApiResponse(
                    mensaje: "Error interno del servidor",
                    exito: false,
                    error: ex.Message
                ));
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout()
        {
            try
            {
                var encryptedToken = Request.Cookies["Cookie"];
                if (string.IsNullOrEmpty(encryptedToken))
                {
                    return BadRequest(new ApiResponse(
                        mensaje: "No se encontró la cookie de sesión",
                        exito: false,
                        error: "Cookie no encontrada"
                    ));
                }

                var token = _encryptionService.Decrypt(encryptedToken, _jwtSecretKey!);
                var result = await _unitOfWork.LoginRepository.LogoutAsync(token);

                Response.Cookies.Delete("Cookie");
                await _sessionCache.RemoveSessionAsync(encryptedToken);

                return Ok(new ApiResponse(
                    mensaje: "Cierre de sesión exitoso",
                    exito: true,
                    datos: result
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Intento de logout no autorizado: {ex.Message}");
                return Unauthorized(new ApiResponse(
                    mensaje: "No autorizado",
                    exito: false,
                    error: ex.Message
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en logout: {ex.Message}");
                return StatusCode(500, new ApiResponse(
                    mensaje: "Error interno del servidor",
                    exito: false,
                    error: ex.Message
                ));
            }
        }
    }
}