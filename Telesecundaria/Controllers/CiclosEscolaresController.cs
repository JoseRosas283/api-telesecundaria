using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.CiclosEscolares;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CiclosEscolaresController : ControllerBase
    {
        private readonly ICiclosEscolaresService _service;

        public CiclosEscolaresController(ICiclosEscolaresService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarCiclos()
        {
            var ciclos = await _service.ListarCiclosAsync();
            if (!ciclos.Any())
                return Ok(new { mensaje = "No hay ciclos escolares registrados." });
            return Ok(ciclos);
        }

        [HttpGet("{claveCiclo}")]
        public async Task<IActionResult> ObtenerPorId(string claveCiclo)
        {
            try
            {
                var ciclo = await _service.ObtenerPorIdAsync(claveCiclo);
                if (ciclo == null)
                    return NotFound(new { mensaje = $"No se encontró el ciclo escolar con clave: {claveCiclo}" });
                return Ok(ciclo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AbrirCiclo([FromBody] CicloEscolarRequestDTO dto)
        {
            try
            {
                await _service.AbrirCicloAsync(dto);
                return StatusCode(201, new { mensaje = "Ciclo escolar abierto correctamente." });
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
