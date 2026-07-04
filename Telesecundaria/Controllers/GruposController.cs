using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Grupos;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GruposController : ControllerBase
    {
        private readonly IGruposService _service;

        public GruposController(IGruposService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarGrupos()
        {
            var grupos = await _service.ListarGruposAsync();
            if (!grupos.Any())
                return Ok(new { mensaje = "No hay grupos registrados." });
            return Ok(grupos);
        }

        [HttpGet("{claveGrupo}")]
        public async Task<IActionResult> ObtenerPorId(string claveGrupo)
        {
            try
            {
                var grupo = await _service.ObtenerPorIdAsync(claveGrupo);
                if (grupo == null)
                    return NotFound(new { mensaje = $"No se encontró el grupo con clave: {claveGrupo}" });
                return Ok(grupo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarGrupo([FromBody] GrupoRequestDTO dto)
        {
            try
            {
                var grupoCreado = await _service.RegistrarGrupoAsync(dto);
                return StatusCode(201, grupoCreado);
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
