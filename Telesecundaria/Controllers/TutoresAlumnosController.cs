using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.TutoresAlumnos;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TutoresAlumnosController : ControllerBase
    {
        private readonly ITutoresAlumnosService _service;

        public TutoresAlumnosController(ITutoresAlumnosService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarRelaciones()
        {
            var relaciones = await _service.ListarAsync();
            if (!relaciones.Any())
                return Ok(new { mensaje = "No hay relaciones tutor alumno registradas." });
            return Ok(relaciones);
        }

        [HttpGet("{claveAlumno}/{claveTutor}")]
        public async Task<IActionResult> ObtenerPorId(string claveAlumno, string claveTutor)
        {
            try
            {
                var relacion = await _service.ObtenerPorIdAsync(claveAlumno, claveTutor);
                if (relacion == null)
                    return NotFound(new { mensaje = "No se encontró la relación tutor alumno solicitada." });
                return Ok(relacion);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AsignarTutorAlumno([FromBody] TutorAlumnoRequestDTO dto)
        {
            try
            {
                var relacionCreada = await _service.AsignarTutorAlumnoAsync(dto);
                return StatusCode(201, relacionCreada);
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
