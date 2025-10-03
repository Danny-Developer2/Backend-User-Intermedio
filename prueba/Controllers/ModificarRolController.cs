using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prueba.Dto;
using prueba.Interfaces;

namespace prueba.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "USER")]



    public class ModificarRolController : ApiControllBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ModificarRolController> _logger;

        public ModificarRolController(
            IUnitOfWork unitOfWork,
            ILogger<ModificarRolController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> ModificarRol([FromBody] ModificarRolRequest request)
        {
            if (request == null)
            {
                return BadRequest();
            }

            var usuario = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);
            if (usuario == null)
            {
                return NotFound();
            }

            try
            {
                usuario.Roles = request.Roles;
                await _unitOfWork.UserRepository.UpdateUserAsync(usuario);
                await _unitOfWork.Complete();

                _logger.LogInformation($"Roles updated for user {usuario.Email}");
                return Ok(new { message = "Roles updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating roles: {ex.Message}");
                return StatusCode(500, new { message = "Error updating roles" });
            }
        }
    }
}