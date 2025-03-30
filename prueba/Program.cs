
using prueba.Interfaces;
using Microsoft.OpenApi.Models;
using prueba.Repositories;
using prueba.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using prueba.Validators;
using prueba.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
// Configurar OpenAPI/Swagger
// / Configurar OpenAPI/Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Mi API",
        Version = "v1",
        Description = "Ejemplo de API con Swagger en ASP.NET Core"
    });

    // Agregar soporte para autenticación en Swagger
    c.AddSecurityDefinition("Cookie", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = "Cookie",
        Description = "Cookie authentication"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Cookie"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddMemoryCache();

// Agregar DbContext para SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar los servicios de repositorio
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// Agregar controladores
builder.Services.AddControllers();

// Habilitar CORS (si necesitas permitir solicitudes desde otros dominios)
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll", builder =>
//         builder.AllowAnyOrigin()
//             .AllowAnyMethod()
//             .AllowAnyHeader()
//             .AllowCredentials()); 
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder
            .WithOrigins("http://localhost:7600", "https://localhost:7600")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});


// ... existing code ...

// Add AutoMapper configuration
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

// Add these services if not already added
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISessionCacheService, SessionCacheService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ISessionService, SessionService>();
// Add this line with your other service registrations
builder.Services.AddScoped<IEncryptionService, EncryptionService>();

// ... existing code ...
builder.Services.AddScoped<IAsistenciaService, AsistenciaService>();
// ... existing code ...
builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
// ... existing code ...
builder.Services.AddHostedService<AttendanceReportScheduler>();
// ... existing code ...

// ... rest of your services configuration ...



// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(options =>
//     {
//         options.Cookie.Name = "Cookie";
//         options.Cookie.HttpOnly = true;
//         options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//         options.Cookie.SameSite = SameSiteMode.Strict;
//         options.ExpireTimeSpan = TimeSpan.FromDays(7);
//         options.Events.OnRedirectToLogin = context =>
//         {
//             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//             return Task.CompletedTask;
//         };
//     });
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(options =>
//     {
//         options.Cookie.Name = "Cookie";
//         options.Cookie.HttpOnly = true;
//         options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Cambiado de Always a None para desarrollo
//         options.Cookie.SameSite = SameSiteMode.Lax; // Cambiado de Strict a Lax para desarrollo
//         options.ExpireTimeSpan = TimeSpan.FromDays(7);
//         options.Events.OnRedirectToLogin = context =>
//         {
//             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//             return Task.CompletedTask;
//         };
//     });
// ... existing services ...
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKeyString = jwtSettings["SecretKey"];
var secretKey = Encoding.UTF8.GetBytes(secretKeyString!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            try
            {
                var encryptedToken = context.Request.Cookies["Cookie"];
                if (!string.IsNullOrEmpty(encryptedToken))
                {
                    var encryptionService = context.HttpContext.RequestServices
                        .GetRequiredService<IEncryptionService>();
                    var secretKey = builder.Configuration["JwtSettings:SecretKey"]!;
                    var decryptedToken = encryptionService.Decrypt(encryptedToken, secretKey);

                    context.Token = decryptedToken;
                }
            }
            catch (Exception ex)
            {
                context.Fail(ex);
            }
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});



// ... rest of the configuration ...

var app = builder.Build();



// Habilitar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API v1");
        c.RoutePrefix = string.Empty; // Hace que Swagger esté disponible en la raíz
    });
}



// Habilitar redirección HTTPS (comentarlo si no estás usando HTTPS en desarrollo)
// app.UseHttpsRedirection();

// Configurar las rutas de la aplicación
app.UseRouting();

// Configuración de CORS (si es necesario)
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Mapeo de controladores
app.MapControllers();

// Iniciar la aplicación
app.Run();
