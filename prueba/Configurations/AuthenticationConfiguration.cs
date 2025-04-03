using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using prueba.Services;  // Add this line for IEncryptionService
using prueba.Interfaces;
public static class AuthenticationConfiguration
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKeyString = jwtSettings["SecretKey"];
        var secretKey = Encoding.UTF8.GetBytes(secretKeyString!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context => HandleTokenDecryption(context, configuration)
            };
        });

        return services;
    }

    private static Task HandleTokenDecryption(MessageReceivedContext context, IConfiguration configuration)
    {
        try
        {
            var encryptedToken = context.Request.Cookies["Cookie"];
            if (!string.IsNullOrEmpty(encryptedToken))
            {
                var encryptionService = context.HttpContext.RequestServices
                    .GetRequiredService<IEncryptionService>();
                var secretKey = configuration["JwtSettings:SecretKey"]!;
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
}