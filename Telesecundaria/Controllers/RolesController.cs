using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Roles.Request;
using Telesecundaria.Models;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [Authorize(Roles = "Administrativo,Directivo")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRolesService _services;

        public RolesController(IRolesService services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _services.GetAllAsync();
            return Ok(roles ?? new List<RolesEntity>());
        }


        [HttpGet("ConsultarRoles/{claveRol}")]
        public async Task<IActionResult> GetById(string claveRol)
        {
            var roles = await _services.GetByIdAsync(claveRol);

            if (roles == null)
                return NotFound(new { Mensaje = $"No existe la clave: {claveRol}" });

            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RolesCreateRequest request)

        {
            try
            {
                var nuevoRol = await _services.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { claveRol = nuevoRol.ClaveRol }, nuevoRol);
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
