using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Tutores;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TutoresController : ControllerBase
    {
        private readonly ITutoresService _service;

        public TutoresController(ITutoresService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTutores()
        {
            var tutores = await _service.ListarTutoresAsync();
            if (!tutores.Any())
                return Ok(new { mensaje = "No hay tutores registrados." });
            return Ok(tutores);
        }

        [HttpGet("{claveTutor}")]
        public async Task<IActionResult> ObtenerPorId(string claveTutor)
        {
            try
            {
                var tutor = await _service.ObtenerPorIdAsync(claveTutor);
                if (tutor == null)
                    return NotFound(new { mensaje = $"No se encontró el tutor con clave: {claveTutor}" });
                return Ok(tutor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarTutor([FromBody] TutorRequestDTO dto)
        {
            try
            {
                var tutorCreado = await _service.RegistrarTutorAsync(dto);
                return StatusCode(201, tutorCreado);
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

        [HttpPut("{claveTutor}")]
        public async Task<IActionResult> ActualizarTutor(string claveTutor, [FromBody] TutorUpdateDTO dto)
        {
            try
            {
                var tutorActualizado = await _service.ActualizarTutorAsync(claveTutor, dto);
                return Ok(tutorActualizado);
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

        [HttpDelete("{claveTutor}")]
        public async Task<IActionResult> EliminarTutor(string claveTutor)
        {
            try
            {
                await _service.EliminarTutorAsync(claveTutor);
                return Ok(new { mensaje = $"Tutor {claveTutor} desactivado correctamente." });
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
                return BadRequest(new { mensaje = "No se pudo desactivar el tutor", detalle = ex.Message });
            }
        }
    }
}
