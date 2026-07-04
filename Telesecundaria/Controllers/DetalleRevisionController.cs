using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.DetalleRevision;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleRevisionController : ControllerBase
    {
        private readonly IDetalleRevisionService _service;

        public DetalleRevisionController(IDetalleRevisionService service)
        {
            _service = service;
        }

        [HttpGet("revision/{claveRevision}")]
        public async Task<IActionResult> ListarPorRevision(string claveRevision)
        {
            try
            {
                var detalles = await _service.ListarPorRevisionAsync(claveRevision);
                if (!detalles.Any())
                    return Ok(new { mensaje = "No hay documentos registrados para esta revisión." });
                return Ok(detalles);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{claveRevision}/{claveDocAspirante}")]
        public async Task<IActionResult> ObtenerPorId(string claveRevision, string claveDocAspirante)
        {
            try
            {
                var detalle = await _service.ObtenerPorIdAsync(claveRevision, claveDocAspirante);
                if (detalle == null)
                    return NotFound(new { mensaje = "No se encontró el detalle solicitado." });
                return Ok(detalle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarDetalle([FromBody] DetalleRevisionRequestDTO dto)
        {
            try
            {
                await _service.RegistrarDetalleAsync(dto);
                return StatusCode(201, new { mensaje = "Detalle de revisión registrado correctamente." });
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

        [HttpPut("{claveRevision}/{claveDocAspirante}")]
        public async Task<IActionResult> ActualizarDetalle(string claveRevision, string claveDocAspirante, [FromBody] DetalleRevisionUpdateDTO dto)
        {
            try
            {
                var detalleActualizado = await _service.ActualizarDetalleAsync(claveRevision, claveDocAspirante, dto);
                return Ok(new { mensaje = "Detalle de revisión actualizado correctamente.", detalle = detalleActualizado });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }
    }
}
