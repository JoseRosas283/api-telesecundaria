using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Revisiones;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RevisionesController : ControllerBase
    {
        private readonly IRevisionesService _service;

        public RevisionesController(IRevisionesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarRevisiones()
        {
            var revisiones = await _service.ListarRevisionesAsync();
            if (!revisiones.Any())
                return Ok(new { mensaje = "No hay revisiones registradas." });
            return Ok(revisiones);
        }

        [HttpGet("{claveRevision}")]
        public async Task<IActionResult> ObtenerPorId(string claveRevision)
        {
            try
            {
                var revision = await _service.ObtenerPorIdAsync(claveRevision);
                if (revision == null)
                    return NotFound(new { mensaje = $"No se encontró la revisión con clave: {claveRevision}" });
                return Ok(revision);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarRevision([FromBody] RevisionRequestDTO dto)
        {
            try
            {
                var revisionCreada = await _service.ProcesarRevisionAsync(dto);
                return StatusCode(201, revisionCreada);
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

        [HttpPatch("{claveRevision}/cerrar")]
        public async Task<IActionResult> CerrarRevision(string claveRevision)
        {
            try
            {
                var revisionCerrada =  await _service.CerrarRevisionAsync(claveRevision);
                return Ok(new { mensaje = $"Revisión {claveRevision} cerrada correctamente.", revisionCerrada });
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
