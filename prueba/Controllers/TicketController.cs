using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prueba.Dto;
using prueba.Entities;
using prueba.Interfaces;
using prueba.Error;
using System.Security.Claims;

namespace prueba.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todas las rutas
    [Authorize(Roles = "ADMIN,USER")]
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
                // Obtener userId desde el token JWT
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                    return Unauthorized(new ApiResponse("No se encontró el UserId en el token", false));

                var userId = Guid.Parse(userIdClaim);
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
                return Ok(new ApiResponse("Tickets obtenidos exitosamente", true, tickets));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse("Error al obtener tickets", false, null, ex.Message));
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
                    return NotFound(new ApiResponse("Ticket no encontrado", false));

                return Ok(new ApiResponse("Ticket obtenido exitosamente", true, ticket));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse("Error al obtener ticket", false, null, ex.Message));
            }
        }

        // -------------------- Actualizar Ticket --------------------
//    [HttpPut("{id}")]
// public async Task<IActionResult> UpdateTicket(int id, [FromBody] TicketUpdateDto dto)
// {
//     try
//     {
//         var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
//         if (userIdClaim == null)
//             return Unauthorized(new ApiResponse("Usuario no autenticado", false));

//         Guid currentUserId = Guid.Parse(userIdClaim);

//         // Si no se envía AssignedToUserId, usar el usuario del token
//         if (!dto.AssignedToUserId.HasValue)
//             dto.AssignedToUserId = currentUserId;

//         var updated = await _unitOfWork.TicketService.UpdateTicketAsync(id, dto);
//         if (!updated)
//             return NotFound(new ApiResponse("Ticket no encontrado", false));

//         return Ok(new ApiResponse("Ticket actualizado exitosamente", true));
//     }
//     catch (Exception ex)
//     {
//         return BadRequest(new ApiResponse("Error al actualizar ticket", false, null, ex.Message));
//     }
// }


        // -------------------- Eliminar Ticket --------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            try
            {
                var deleted = await _unitOfWork.TicketService.DeleteTicketAsync(id);
                if (!deleted)
                    return NotFound(new ApiResponse("Ticket no encontrado", false));

                return Ok(new ApiResponse("Ticket eliminado exitosamente", true));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse("Error al eliminar ticket", false, null, ex.Message));
            }
        }

        // -------------------- Tickets Cancelados o Abiertos --------------------
        [HttpGet("status/cancelled-or-open")]
        public async Task<IActionResult> GetCancelledOrOpenTickets()
        {
            try
            {
                var tickets = await _unitOfWork.TicketService.GetTicketsByStatusAsync(
                    new List<TicketStatus> { TicketStatus.Cancelled, TicketStatus.Open }
                );

                return Ok(new ApiResponse("Tickets Cancelados o Abiertos obtenidos exitosamente", true, tickets));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse("Error al obtener tickets", false, null, ex.Message));
            }
        }

        // -------------------- Tickets InProgress (ordenados por prioridad) --------------------
        [HttpGet("status/inprogress")]
        public async Task<IActionResult> GetInProgressTickets()
        {
            try
            {
                var tickets = await _unitOfWork.TicketService.GetTicketsByStatusAsync(
                    new List<TicketStatus> { TicketStatus.InProgress },
                    orderByPriority: true
                );

                return Ok(new ApiResponse("Tickets InProgress obtenidos exitosamente", true, tickets));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse("Error al obtener tickets", false, null, ex.Message));
            }
        }

        // -------------------- Tickets Cerrados o Resueltos --------------------
        [HttpGet("status/closed-or-resolved")]
        public async Task<IActionResult> GetClosedOrResolvedTickets()
        {
            try
            {
                var tickets = await _unitOfWork.TicketService.GetTicketsByStatusAsync(
                    new List<TicketStatus> { TicketStatus.Closed, TicketStatus.Resolved }
                );

                return Ok(new ApiResponse("Tickets Cerrados o Resueltos obtenidos exitosamente", true, tickets));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse("Error al obtener tickets", false, null, ex.Message));
            }
        }
        [HttpGet("stats/average-resolution")]
public async Task<IActionResult> GetAverageResolutionTime()
{
    var avgTime = await _unitOfWork.TicketService.GetAverageResolutionTimeAsync();
    return Ok(new { averageHours = avgTime });
}

[HttpGet("stats/average-closed/{userId}")]
public async Task<IActionResult> GetAverageClosedTimeByUser(Guid userId)
{
    var avgTime = await _unitOfWork.TicketService.GetAverageClosedTimeByUserAsync(userId);
    return Ok(new { userId, averageHours = avgTime });
}
[HttpGet("my-tickets")]
[Authorize]
public async Task<IActionResult> GetMyTickets()
{
    try
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized(new ApiResponse("Usuario no autenticado", false));

        Guid currentUserId = Guid.Parse(userIdClaim);

        // Obtener solo los tickets asignados a este usuario
        var tickets = await _unitOfWork.TicketService.GetTicketsByAssignedUserAsync(currentUserId);

        return Ok(new ApiResponse("Tickets obtenidos exitosamente", true, tickets));
    }
    catch (Exception ex)
    {
        return BadRequest(new ApiResponse("Error al obtener tickets", false, null, ex.Message));
    }
}
[HttpPut("{id}")]
[Authorize]
public async Task<IActionResult> UpdateMyTicket(int id, [FromBody] TicketUpdateDto dto)
{
    try
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null)
            return Unauthorized(new ApiResponse("Usuario no autenticado", false));

        Guid currentUserId = Guid.Parse(userIdClaim);

        // Pasar usuario al servicio para validar que solo pueda modificar sus tickets
        var updated = await _unitOfWork.TicketService.UpdateTicketForAssignedUserAsync(id, dto, currentUserId);

        if (!updated)
            return NotFound(new ApiResponse("Ticket no encontrado o no asignado a este usuario", false));

        return Ok(new ApiResponse("Ticket actualizado exitosamente", true));
    }
    catch (Exception ex)
    {
        return BadRequest(new ApiResponse("Error al actualizar ticket", false, null, ex.Message));
    }
}


    }
}
