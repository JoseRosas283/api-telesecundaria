using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.TipoNotificaciones;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoNotificacionesController : ControllerBase
    {
        private readonly ITipoNotificacionesService _service;

        public TipoNotificacionesController(ITipoNotificacionesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var tipos = await _service.ListarAsync();
            if (!tipos.Any())
                return Ok(new { mensaje = "No hay tipos de notificación registrados." });
            return Ok(tipos);
        }

        [HttpGet("{clave}")]
        public async Task<IActionResult> ObtenerPorId(string clave)
        {
            try
            {
                var tipo = await _service.ObtenerPorIdAsync(clave);
                if (tipo == null)
                    return NotFound(new { mensaje = $"No se encontró el tipo de notificación con clave: {clave}" });
                return Ok(tipo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] TipoNotificacionRequestDTO dto)
        {
            try
            {
                var creado = await _service.RegistrarAsync(dto);
                return StatusCode(201, creado);
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
