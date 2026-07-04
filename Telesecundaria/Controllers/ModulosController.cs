using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Modulo.Request;
using Telesecundaria.DTOs.Modulo.Response;
using Telesecundaria.Models;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [Authorize(Roles = "Administrativo,Directivo")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ModulosController : ControllerBase
    {
        private readonly IModuloService _service;

        public ModulosController(IModuloService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var modulos = await _service.GetAllAsync();
            return Ok(modulos ?? new List<ModuloResponse>());
        }

        [HttpGet("ConsultarModulo/{claveModulo}")]
        public async Task<IActionResult> GetById(string claveModulo)
        {
            var modulo = await _service.GetByIdAsync(claveModulo);

            if (modulo == null)
                return NotFound(new { Mensaje = $"No existe el módulo con clave {claveModulo}" });

            return Ok(modulo);
        }

        [HttpGet("ConsultarPorNombre/{nombreModulo}")]
        public async Task<IActionResult> GetByNombre(string nombreModulo)
        {
            var modulo = await _service.GetByNombreAsync(nombreModulo);

            if (modulo == null)
                return NotFound(new { Mensaje = $"No existe el módulo con nombre {nombreModulo}" });

            return Ok(modulo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ModuloCreateRequest request)
        {
            try
            {
                var nuevoModulo = await _service.CreateAsync(request);

                return CreatedAtAction(nameof(GetById),
                    new { claveModulo = nuevoModulo.ClaveModulo }, nuevoModulo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno: {ex.Message}" });
            }
        }

    }
}
