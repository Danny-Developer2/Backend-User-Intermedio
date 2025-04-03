using Microsoft.Extensions.DependencyInjection;
using prueba.Data;
using prueba.Interfaces;
using prueba.Repositories;
using prueba.Services;


public static class DependencyInjectionConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ILoginRepository, LoginRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISessionCacheService, SessionCacheService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IAsistenciaService, AsistenciaService>();
        services.AddScoped<IWhatsAppService, WhatsAppService>();
        services.AddHostedService<AttendanceReportScheduler>();

        return services;
    }
}