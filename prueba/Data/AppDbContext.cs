using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using prueba.Entities;

namespace prueba.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
            
        }
        public required DbSet<Producto> Productos { get; set; }

        public required DbSet<User> Users { get; set; }

        public required DbSet<UserSession> UserSessions { get; set; }

        public required DbSet<RegisterAsistencia> RegisterAsistencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(e => e.Roles)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!)!);
        }



    }
}