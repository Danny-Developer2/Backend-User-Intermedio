using prueba.Data;
using Microsoft.EntityFrameworkCore;
using prueba.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Service Configuration
// Service Configuration
builder.Services
    .AddSwaggerConfiguration()
    .AddMemoryCache()
    .AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddControllers();

// Add Validators
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();

// ... rest of your code remains the same ...

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder
            .WithOrigins(
                "http://localhost:7600",
                "https://localhost:7600",
                "http://localhost:8081",
                "exp://10.70.197.184:8081",
                "http://10.70.197.184:7600",
                "http://10.70.197.184:8081",
                "http://10.70.197.184:5000/",
                "http://192.168.100.6:8081"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add Application Services
builder.Services.AddApplicationServices();

// Add Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Middleware Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Users API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseRouting()
   .UseCors("AllowAll")
   .UseAuthentication()
   .UseAuthorization();

app.MapControllers();

app.Run();