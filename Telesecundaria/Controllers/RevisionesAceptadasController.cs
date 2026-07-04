using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.RevisionesAceptadas;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevisionesAceptadasController : ControllerBase
    {
        private readonly IRevisionesAceptadasService _service;

        public RevisionesAceptadasController(IRevisionesAceptadasService service)
        {
            _service = service;
        }

        [HttpGet("pendientes")]
        public async Task<IActionResult> ListarPendientes()
        {
            var pendientes = await _service.ListarPendientesAsync();
            if (!pendientes.Any())
                return Ok(new { mensaje = "No hay aspirantes pendientes de cita." });
            return Ok(pendientes);
        }

        [HttpGet("{claveRevision}")]
        public async Task<IActionResult> ObtenerPorId(string claveRevision)
        {
            try
            {
                var aceptada = await _service.ObtenerPorIdAsync(claveRevision);
                if (aceptada == null)
                    return NotFound(new { mensaje = $"No se encontró un registro aceptado con la revisión: {claveRevision}" });
                return Ok(aceptada);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarAceptacion([FromBody] RevisionAceptadaRequestDTO dto)
        {
            try
            {
                await _service.RegistrarAceptacionAsync(dto);
                return StatusCode(201, new { mensaje = "Aspirante encolado al buffer de citas correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }
    }
}
