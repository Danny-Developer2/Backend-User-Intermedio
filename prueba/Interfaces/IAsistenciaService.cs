using prueba.Dto;
using prueba.Entities;

namespace prueba.Services
{
    public interface IAsistenciaService
    {
        Task<RegisterAsistencia> RegisterAsistenciaAsync(RegisterAsistenciaDTO dto);
        Task SendDailyReportWhatsApp();
    }
}