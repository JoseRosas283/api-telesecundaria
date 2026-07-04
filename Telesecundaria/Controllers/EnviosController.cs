using Microsoft.AspNetCore.Mvc;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnviosController : ControllerBase
    {
        private readonly IEnviosService _service;

        public EnviosController(IEnviosService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var envios = await _service.ListarTodosAsync();
            if (!envios.Any())
                return Ok(new { mensaje = "No hay envíos registrados." });
            return Ok(envios);
        }

        [HttpGet("{clave}")]
        public async Task<IActionResult> ObtenerPorId(string clave)
        {
            try
            {
                var envio = await _service.ObtenerPorIdAsync(clave);
                if (envio == null)
                    return NotFound(new { mensaje = $"No se encontró el envío con clave: {clave}" });
                return Ok(envio);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost("procesar-pendientes")]
        public async Task<IActionResult> ProcesarPendientes()
        {
            try
            {
                var resultado = await _service.ProcesarPendientesAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }
    }
}
