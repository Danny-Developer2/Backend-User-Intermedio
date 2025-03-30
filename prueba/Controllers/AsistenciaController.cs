using Microsoft.AspNetCore.Mvc;
using prueba.Dto;
using prueba.Services;

namespace prueba.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciaController : ApiControllBase
    {
        private readonly IAsistenciaService _asistenciaService;

        public AsistenciaController(IAsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsistencia([FromBody] RegisterAsistenciaDTO dto)
        {
            var result = await _asistenciaService.RegisterAsistenciaAsync(dto);
            return Ok(result);
        }


        [HttpPost("send-report")]
        public async Task<IActionResult> SendReport()
        {
            await _asistenciaService.SendDailyReportWhatsApp();
            return Ok(new { message = "Reporte enviado exitosamente" });
        }
    }
}