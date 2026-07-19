using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Entregas;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntregasController : ControllerBase
    {
        private readonly IEntregasService _service;

        public EntregasController(IEntregasService service)
        {
            _service = service;
        }

        // Inicializa la entrega a partir de una cita 'Programada' (1 vez por cita)
        [HttpPost]
        public async Task<IActionResult> InicializarEntrega([FromBody] EntregaRequestDTO dto)
        {
            try
            {
                var resultado = await _service.InicializarEntregaAsync(dto);
                return StatusCode(201, resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", detalle = ex.Message });
            }
        }
    }
}
