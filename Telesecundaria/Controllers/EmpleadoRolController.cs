using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.EmpleadoRol.Request;
using Telesecundaria.Models;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [Authorize(Roles = "Administrativo,Directivo")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmpleadoRolController : ControllerBase
    {
        private readonly IEmpleadoRolService _service;

        public EmpleadoRolController(IEmpleadoRolService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var rol = await _service.GetAllAsync();
            return Ok(rol ?? new List<EmpleadoRolEntity>());
        }

        [HttpGet("ConsultarRol/{claveRol}")]
        public async Task<IActionResult> GetById(string claveRol)
        {
            var rol = await _service.GetByIdAsync(claveRol);

            if (rol == null)
                return NotFound(new { Mensaje = $"No existe la clave rol-Empleado {claveRol}" });

            return Ok(rol);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RolCreateRequest request)
        {
            try
            {
                var nuevoRolEmpleado = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { claveRol = nuevoRolEmpleado.ClaveRol }, nuevoRolEmpleado);
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
