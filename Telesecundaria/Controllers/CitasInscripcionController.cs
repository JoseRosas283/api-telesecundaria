using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.CitasInscripcion;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasInscripcionController : ControllerBase
    {
        private readonly ICitasInscripcionService _service;

        public CitasInscripcionController(ICitasInscripcionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarCitas()
        {
            var citas = await _service.ListarCitasAsync();
            if (!citas.Any())
                return Ok(new { mensaje = "No hay citas registradas." });

            return Ok(citas);
        }

        [HttpGet("{claveCita}")]
        public async Task<IActionResult> ObtenerPorId(string claveCita)
        {
            try
            {
                var cita = await _service.ObtenerPorIdAsync(claveCita);
                if (cita == null)
                    return NotFound(new { mensaje = $"No se encontró la cita con clave: {claveCita}" });

                return Ok(cita);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgendarCita([FromBody] CitaInscripcionRequestDTO dto)
        {
            try
            {
                var citaCreada = await _service.AgendarCitaAsync(dto);
                return StatusCode(201, citaCreada);
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
