using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Publicaciones;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicacionesController : ControllerBase
    {
        private readonly IPublicacionesService _service;

        public PublicacionesController(IPublicacionesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPublicaciones()
        {
            var publicaciones = await _service.ListarPublicacionesAsync();

            if (!publicaciones.Any())
                return Ok(new { mensaje = "No hay publicaciones registradas." });

            return Ok(publicaciones);
        }

        [HttpGet("{clavePublicacion}")]
        public async Task<IActionResult> ObtenerPorId(string clavePublicacion)
        {
            try
            {
                var publicacion = await _service.ObtenerPorIdAsync(clavePublicacion);

                if (publicacion == null)
                    return NotFound(new { mensaje = $"No se encontró la publicación con clave: {clavePublicacion}" });

                return Ok(publicacion);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPublicacion([FromBody] PublicacionRequestDTO dto)
        {
            try
            {
                var publicacionCreada = await _service.RegistrarPublicacionAsync(dto);
                return StatusCode(201, publicacionCreada);
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

        [HttpPut("{clavePublicacion}")]
        public async Task<IActionResult> ActualizarPublicacion(string clavePublicacion, [FromBody] PublicacionUpdateDTO dto)
        {
            try
            {
                await _service.ActualizarPublicacionAsync(clavePublicacion, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = "Error de validación", detalle = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        [HttpDelete("{clavePublicacion}")]
        public async Task<IActionResult> EliminarPublicacion(string clavePublicacion)
        {
            try
            {
                await _service.EliminarPublicacionAsync(clavePublicacion);
                return Ok(new { mensaje = $"Publicación {clavePublicacion} eliminada correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "No se pudo eliminar la publicación", detalle = ex.Message });
            }
        }
    }
}
