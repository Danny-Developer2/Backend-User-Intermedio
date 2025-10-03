using Microsoft.AspNetCore.Mvc;
using prueba.Dto;
using prueba.Interfaces;
using prueba.Entities;
using AutoMapper;
using prueba.Error;
using Microsoft.AspNetCore.Authorization;

namespace prueba.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ApiControllBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // -------------------- Crear Ticket --------------------
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketCreateDto dto)
        {
            try
            {
                // Aquí normalmente obtienes el ID del usuario logueado
                var userId = dto.CreatedByUserId; // <-- Cambiar según tu auth; // <-- Cambiar según tu auth
                var ticket = await _unitOfWork.TicketService.CreateTicketAsync(dto, userId);

                return Ok(new ApiResponse(
                    mensaje: "Ticket creado exitosamente",
                    exito: true,
                    datos: ticket
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(
                    mensaje: "Error al crear el ticket",
                    exito: false,
                    datos: null,
                    error: ex.Message
                ));
            }
        }

        // -------------------- Obtener todos los Tickets --------------------
        [HttpGet]
        public async Task<IActionResult> GetAllTickets()
        {
            try
            {
                var tickets = await _unitOfWork.TicketService.GetAllTicketsAsync();
                return Ok(new ApiResponse(
                    mensaje: "Tickets obtenidos exitosamente",
                    exito: true,
                    datos: tickets
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(
                    mensaje: "Error al obtener tickets",
                    exito: false,
                    datos: null,
                    error: ex.Message
                ));
            }
        }

        // -------------------- Obtener un Ticket por Id --------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int id)
        {
            try
            {
                var ticket = await _unitOfWork.TicketService.GetTicketByIdAsync(id);
                if (ticket == null)
                    return NotFound(new ApiResponse(
                        mensaje: "Ticket no encontrado",
                        exito: false,
                        datos: null
                    ));

                return Ok(new ApiResponse(
                    mensaje: "Ticket obtenido exitosamente",
                    exito: true,
                    datos: ticket
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(
                    mensaje: "Error al obtener ticket",
                    exito: false,
                    datos: null,
                    error: ex.Message
                ));
            }
        }

        // -------------------- Actualizar Ticket --------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] TicketUpdateDto dto)
        {
            try
            {
                var updated = await _unitOfWork.TicketService.UpdateTicketAsync(id, dto);
                if (!updated)
                    return NotFound(new ApiResponse(
                        mensaje: "Ticket no encontrado",
                        exito: false,
                        datos: null
                    ));

                return Ok(new ApiResponse(
                    mensaje: "Ticket actualizado exitosamente",
                    exito: true,
                    datos: null
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(
                    mensaje: "Error al actualizar ticket",
                    exito: false,
                    datos: null,
                    error: ex.Message
                ));
            }
        }

        // -------------------- Eliminar Ticket --------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            try
            {
                var deleted = await _unitOfWork.TicketService.DeleteTicketAsync(id);
                if (!deleted)
                    return NotFound(new ApiResponse(
                        mensaje: "Ticket no encontrado",
                        exito: false,
                        datos: null
                    ));

                return Ok(new ApiResponse(
                    mensaje: "Ticket eliminado exitosamente",
                    exito: true,
                    datos: null
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(
                    mensaje: "Error al eliminar ticket",
                    exito: false,
                    datos: null,
                    error: ex.Message
                ));
            }
        }
    }
}
