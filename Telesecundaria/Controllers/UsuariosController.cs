using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Usuarios.Request;
using Telesecundaria.Models;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [Authorize(Roles = "Administrativo,Directivo")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _service.GetAllAsync();
            // Retorna lista vacía en lugar de null para evitar errores en el cliente
            return Ok(usuarios ?? new List<UsuariosEntity>());
        }

        // GET: api/Usuarios/ConsultarUsuario/USR-001
        [HttpGet("ConsultarUsuario/{claveUsuario}")]
        public async Task<IActionResult> GetById(string claveUsuario)
        {
            var usuario = await _service.GetByIdAsync(claveUsuario);
            if (usuario == null)
                return NotFound(new { Mensaje = $"No existe el usuario con clave {claveUsuario}" });

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateRequest request)
        {
            try
            {
                // La lógica dura de validación ya reside en el Service
                var nuevoUsuario = await _service.CreateAsync(request);

                // Redirige a la acción de consulta
                return CreatedAtAction(nameof(GetById),
                    new { claveUsuario = nuevoUsuario.ClaveUsuario }, nuevoUsuario);
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

        // PUT: api/Usuarios/actualizarUsuario/USR-001
        [HttpPut("actualizarUsuario/{claveUsuario}")]
        public async Task<IActionResult> Update(string claveUsuario, [FromBody] UsuarioUpdateRequest request)
        {
            try
            {
                await _service.UpdateAsync(claveUsuario, request);

                // Buscamos el usuario actualizado para retornarlo (opcional, similar a tu ejemplo)
                var actualizado = await _service.GetByIdAsync(claveUsuario);
                return Ok(actualizado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno: {ex.Message}" });
            }
        }

        // DELETE: api/Usuarios/eliminarUsuario/USR-001
        [HttpDelete("eliminarUsuario/{claveUsuario}")]
        public async Task<IActionResult> Delete(string claveUsuario)
        {
            try
            {
                await _service.DeleteAsync(claveUsuario);
                return NoContent(); // 204 No Content para eliminaciones exitosas
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Para errores de integridad (si tiene publicaciones)
                return Conflict(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
        }
    }
}
