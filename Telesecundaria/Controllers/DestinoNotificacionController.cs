using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.DestinoNotificacion;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DestinoNotificacionController : ControllerBase
    {
        private readonly IDestinoNotificacionService _service;

        public DestinoNotificacionController(IDestinoNotificacionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var destinos = await _service.ListarAsync();
            if (!destinos.Any())
                return Ok(new { mensaje = "No hay destinos de notificación configurados." });
            return Ok(destinos);
        }

        [HttpGet("{clave}")]
        public async Task<IActionResult> ObtenerPorId(string clave)
        {
            try
            {
                var destino = await _service.ObtenerPorIdAsync(clave);
                if (destino == null)
                    return NotFound(new { mensaje = $"No se encontró el destino con clave: {clave}" });
                return Ok(destino);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] DestinoNotificacionRequestDTO dto)
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
