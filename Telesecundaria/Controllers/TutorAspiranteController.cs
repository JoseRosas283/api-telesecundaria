using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.TutorAspirante;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TutorAspiranteController : ControllerBase
    {
        private readonly ITutorAspiranteService _service;

        public TutorAspiranteController(ITutorAspiranteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTutores()
        {
            var tutores = await _service.ListarTutoresAsync();

            if (!tutores.Any())
                return Ok(new { mensaje = "No hay tutores aspirantes registrados." });

            return Ok(tutores);
        }

        [HttpGet("{claveTutorAspirante}")]
        public async Task<IActionResult> ObtenerPorId(string claveTutorAspirante)
        {
            var tutor = await _service.ObtenerPorIdAsync(claveTutorAspirante);

            if (tutor == null)
                return NotFound(new { mensaje = $"No se encontró el tutor con clave: {claveTutorAspirante}" });

            return Ok(tutor);
        }

        [HttpGet("curp/{curp}")]
        public async Task<IActionResult> ObtenerPorCurp(string curp)
        {
            var tutor = await _service.ObtenerPorCurpAsync(curp);

            if (tutor == null)
                return NotFound(new { mensaje = $"No se encontró ningún tutor con CURP: {curp}" });

            return Ok(tutor);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarTutor([FromBody] TutorAspiranteRequestDTO dto)
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
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        [HttpPut("{claveTutorAspirante}")]
        public async Task<IActionResult> ActualizarTutor(string claveTutorAspirante, [FromBody] TutorAspiranteUpdateDTO dto)
        {
            try
            {
                await _service.ActualizarTutorAsync(claveTutorAspirante, dto);
                return Ok(new
                {
                    mensaje = "Registro del tutor y su dirección actualizados correctamente.",
                    clave = claveTutorAspirante
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = "Error de validación", detalle = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = "No encontrado", detalle = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        [HttpDelete("{claveTutorAspirante}")]
        public async Task<IActionResult> EliminarTutor(string claveTutorAspirante)
        {
            try
            {
                await _service.EliminarTutorAsync(claveTutorAspirante);
                return Ok(new { mensaje = $"Tutor aspirante {claveTutorAspirante} eliminado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = "Error en los datos de la petición", detalle = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = "No encontrado", detalle = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "No se pudo eliminar el tutor aspirante", detalle = ex.Message });
            }
        }
    }
}
