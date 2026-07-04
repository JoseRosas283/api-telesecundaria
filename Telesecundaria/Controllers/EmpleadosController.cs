using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Empleados.Request;
using Telesecundaria.Models;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [Authorize(Roles = "Administrativo,Directivo")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly IEmpleadoService _service;

        public EmpleadosController(IEmpleadoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var empleados = await _service.GetAllAsync();
            return Ok(empleados ?? new List<EmpleadosEntity>());
        }

        [HttpGet("ConsultarEmpleado/{claveEmpleado}")]
        public async Task<IActionResult> GetById(string claveEmpleado)
        {
            var empleado = await _service.GetByIdAsync(claveEmpleado);

            if (empleado == null)
                return NotFound(new { Mensaje = $"No existe el empleado con clave: {claveEmpleado}" });

            return Ok(empleado);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmpleadoCreateRequest request)
        {
            try
            {
                var nuevoEmpleado = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById),
                    new { claveEmpleado = nuevoEmpleado.ClaveEmpleado }, nuevoEmpleado);
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

        [HttpPut("actualizarEmpleado/{claveEmpleado}")]
        public async Task<IActionResult> Update(string claveEmpleado, [FromBody] EmpleadoUpdateRequest request)
        {
            try
            {
                await _service.UpdateAsync(claveEmpleado, request);
                var actualizado = await _service.GetByIdAsync(claveEmpleado);
                return Ok(actualizado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [HttpDelete("eliminarEmpleado/{claveEmpleado}")]
        public async Task<IActionResult> Delete(string claveEmpleado)
        {
            try
            {
                await _service.DeleteAsync(claveEmpleado);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {

                return Conflict(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }
}
