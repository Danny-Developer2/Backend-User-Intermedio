using prueba.Data;
using prueba.Dto;
using prueba.Entities;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace prueba.Services
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly AppDbContext _context;
        private readonly IWhatsAppService _whatsAppService;

        public AsistenciaService(AppDbContext context, IWhatsAppService whatsAppService)
        {
            _context = context;
            _whatsAppService = whatsAppService;
        }

        public async Task<RegisterAsistencia> RegisterAsistenciaAsync(RegisterAsistenciaDTO dto)
        {
            // Find user by full name using ToLower() which can be translated to SQL
            var user = await _context.Users
                .FirstOrDefaultAsync(u => (u.FirstName.ToLower() + " " + u.LastName.ToLower()) == dto.FullName.ToLower());

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var asistencia = new RegisterAsistencia
            {
                UserId = user.Id,
                Fecha = dto.Fecha,
                Asistencia = dto.Asistencia,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddDays(1)
            };

            await _context.RegisterAsistencias.AddAsync(asistencia);
            await _context.SaveChangesAsync();
            return asistencia;
        }

        public async Task SendDailyReportWhatsApp()
        {
            var today = DateTime.Today;
            var currentTime = DateTime.Now;

            // Get all active users
            var allUsers = await _context.Users.ToListAsync();

            // Get ONLY today's attendance records for today's date
            var asistencias = await _context.RegisterAsistencias
                .Where(a => a.CreatedAt.Date == today && a.Fecha.Date == today)  // Check both dates
                .Include(a => a.User)
                .ToListAsync();

            // Rest of the code remains the same...
            var registeredUserIds = asistencias.Select(a => a.UserId).ToList();
            var missingUsers = allUsers.Where(u => !registeredUserIds.Contains(u.Id));

            foreach (var user in missingUsers)
            {
                var defaultAsistencia = new RegisterAsistencia
                {
                    UserId = user.Id,
                    User = user,
                    Fecha = today,
                    Asistencia = false,
                    CreatedAt = currentTime,
                    ExpiresAt = currentTime.AddDays(1)
                };

                asistencias.Add(defaultAsistencia);
                await _context.RegisterAsistencias.AddAsync(defaultAsistencia);
            }

            if (missingUsers.Any())
            {
                await _context.SaveChangesAsync();
            }

            asistencias = asistencias.OrderBy(a => a.User.FirstName).ToList();
            var report = GenerateReport(asistencias);
            await _whatsAppService.SendMessage(report);
        }
        private string GenerateReport(List<RegisterAsistencia> asistencias)
        {
            var sb = new StringBuilder();
            sb.AppendLine("📊 Daily Attendance Report");
            sb.AppendLine($"Date: {DateTime.Now:dd/MM/yyyy}");
            sb.AppendLine("------------------------");

            foreach (var asistencia in asistencias)
            {
                sb.AppendLine($"👤 {asistencia.User.FirstName}: {(asistencia.Asistencia ? "✅ Present" : "❌ Absent")}");
            }

            return sb.ToString();
        }
    }
}