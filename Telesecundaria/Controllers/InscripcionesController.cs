using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Inscripciones;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscripcionesController : ControllerBase
    {
        private readonly IInscripcionesService _service;

        public InscripcionesController(IInscripcionesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarInscripciones()
        {
            var inscripciones = await _service.ListarInscripcionesAsync();
            if (!inscripciones.Any())
                return Ok(new { mensaje = "No hay inscripciones registradas." });
            return Ok(inscripciones);
        }

        [HttpGet("{claveInscripcion}")]
        public async Task<IActionResult> ObtenerPorId(string claveInscripcion)
        {
            try
            {
                var inscripcion = await _service.ObtenerPorIdAsync(claveInscripcion);
                if (inscripcion == null)
                    return NotFound(new { mensaje = $"No se encontró la inscripción con clave: {claveInscripcion}" });
                return Ok(inscripcion);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RealizarInscripcion([FromBody] InscripcionRequestDTO dto)
        {
            try
            {
                await _service.RealizarInscripcionAsync(dto);
                return StatusCode(201, new { mensaje = "Inscripción realizada correctamente." });
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
