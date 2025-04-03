using Microsoft.AspNetCore.Mvc;
using prueba.Dto;
using prueba.Services;
using prueba.Interfaces;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using prueba.Error;

namespace prueba.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistenciaController(IUnitOfWork unitOfWork) : ApiControllBase
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
      
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsistencia([FromBody] RegisterAsistenciaDTO dto)
        {
            try
            {


                // var result = await _asistenciaService.RegisterAsistenciaAsync(dto);
                var result = await _unitOfWork.AsistenciaService.RegisterAsistenciaAsync(dto);

                // if (result == null)
                // {
                //     return BadRequest(new ApiResponse(
                //         mensaje: "No se pudo registrar la asistencia",
                //         exito: false,
                //         datos: null,
                //         error: "El Usuario no existe"
                //     ));
                // }

                return Ok(new ApiResponse(
                    mensaje: "Asistencia registrada exitosamente",
                    exito: true,
                    datos: result
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(
                    mensaje: "Error al registrar la asistencia",
                    exito: false,
                    datos: null,
                    error: ex.Message
                ));
            }

        }


        [HttpPost("send-report")]
        public async Task<IActionResult> SendReport()
        {

            try
            {

                // await _asistenciaService.SendDailyReportWhatsApp();
                await _unitOfWork.AsistenciaService.SendDailyReportWhatsApp();

                return Ok(new ApiResponse(
                    mensaje: "Reporte diario enviado exitosamente",
                    exito: true,
                    datos: null
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al enviar el reporte", error = ex.Message });
            }
        }
    }
}