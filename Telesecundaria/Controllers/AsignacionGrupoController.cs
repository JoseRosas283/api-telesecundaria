using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.AsignacionGrupo;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionGrupoController : ControllerBase
    {
        private readonly IAsignacionGrupoService _service;

        public AsignacionGrupoController(IAsignacionGrupoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarAsignaciones()
        {
            var asignaciones = await _service.ListarAsignacionesAsync();
            if (!asignaciones.Any())
                return Ok(new { mensaje = "No hay asignaciones registradas." });
            return Ok(asignaciones);
        }

        [HttpGet("{claveAsignacion}")]
        public async Task<IActionResult> ObtenerPorId(string claveAsignacion)
        {
            try
            {
                var asignacion = await _service.ObtenerPorIdAsync(claveAsignacion);
                if (asignacion == null)
                    return NotFound(new { mensaje = $"No se encontró la asignación con clave: {claveAsignacion}" });
                return Ok(asignacion);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AsignarAlumnoGrupo([FromBody] AsignacionGrupoRequestDTO dto)
        {
            try
            {
                await _service.AsignarAlumnoGrupoAsync(dto);
                return StatusCode(201, new { mensaje = "Alumno asignado al grupo correctamente." });
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
