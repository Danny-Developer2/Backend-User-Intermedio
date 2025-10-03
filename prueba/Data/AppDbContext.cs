// using System.Text.Json;
// using Microsoft.EntityFrameworkCore;
// using prueba.Entities;

// namespace prueba.Data
// {
//     public class AppDbContext : DbContext
//     {
//         public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
            
//         }
//         public required DbSet<Producto> Productos { get; set; }

//         public required DbSet<User> Users { get; set; }

//         public required DbSet<UserSession> UserSessions { get; set; }

//         public required DbSet<RegisterAsistencia> RegisterAsistencias { get; set; }

//         public required DbSet<Ticket> Tickets {get; set;}

//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);

//             modelBuilder.Entity<User>()
//                 .Property(e => e.Roles)
//                 .HasConversion(
//                     v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
//                     v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!)!);

//             // Relación Ticket -> Usuario que lo creó (obligatoria)
//     modelBuilder.Entity<Ticket>()
//         .HasOne(t => t.CreatedBy)
//         .WithMany(u => u.TicketsCreated)
//         .HasForeignKey(t => t.CreatedByUserId)
//         .OnDelete(DeleteBehavior.Restrict);

//     // Relación Ticket -> Usuario asignado (opcional)
//     modelBuilder.Entity<Ticket>()
//         .HasOne(t => t.AssignedTo)
//         .WithMany(u => u.TicketsAssigned)
//         .HasForeignKey(t => t.AssignedToUserId)
//         .OnDelete(DeleteBehavior.Restrict);

//     // Relación TicketComment -> Usuario
//     modelBuilder.Entity<TicketComment>()
//         .HasOne(tc => tc.User)
//         .WithMany()
//         .HasForeignKey(tc => tc.UserId)
//         .OnDelete(DeleteBehavior.Restrict);

//     // Relación TicketComment -> Ticket
//     modelBuilder.Entity<TicketComment>()
//         .HasOne(tc => tc.Ticket)
//         .WithMany(t => t.Comments)
//         .HasForeignKey(tc => tc.TicketId)
//         .OnDelete(DeleteBehavior.Cascade);

//         }



//     }
// }



using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using prueba.Entities;

namespace prueba.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public required DbSet<Producto> Productos { get; set; }

        public required DbSet<User> Users { get; set; }

        public required DbSet<UserSession> UserSessions { get; set; }

        public required DbSet<RegisterAsistencia> RegisterAsistencias { get; set; }

        public required DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Roles con ValueComparer para listas
            modelBuilder.Entity<User>()
                .Property(e => e.Roles)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!)!
                )
                .Metadata.SetValueComparer(
                    new ValueComparer<List<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()
                    )
                );

            // Relación Ticket -> Usuario que lo creó (obligatoria)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.CreatedBy)
                .WithMany(u => u.TicketsCreated)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Ticket -> Usuario asignado (opcional)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedTo)
                .WithMany(u => u.TicketsAssigned)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación TicketComment -> Usuario
            modelBuilder.Entity<TicketComment>()
                .HasOne(tc => tc.User)
                .WithMany()
                .HasForeignKey(tc => tc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación TicketComment -> Ticket
            modelBuilder.Entity<TicketComment>()
                .HasOne(tc => tc.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(tc => tc.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
