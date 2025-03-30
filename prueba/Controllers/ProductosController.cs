using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prueba.Entities;
using prueba.Interfaces;

namespace prueba.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Authorize(Roles = "ADMIN,USER")]

    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _repo;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(
            IProductoRepository repo,
            ILogger<ProductosController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            try
            {
                var productos = await _repo.ObtenerTodosAsync();
                if (productos == null || !productos.Any())
                {
                    _logger.LogInformation("No products found");
                    return NotFound(new { message = "No products found" });
                }

                return Ok(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}