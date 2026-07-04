using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.GaleriaImagenes;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GaleriaImagenesController : ControllerBase
    {
        private readonly IGaleriaImagenesService _service;

        public GaleriaImagenesController(IGaleriaImagenesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarImagenes()
        {
            var imagenes = await _service.ListarImagenesAsync();

            if (!imagenes.Any())
                return Ok(new { mensaje = "No hay imágenes registradas en la galería." });

            return Ok(imagenes);
        }

        [HttpGet("{claveImagen}")]
        public async Task<IActionResult> ObtenerPorId(string claveImagen)
        {
            try
            {
                var imagen = await _service.ObtenerPorIdAsync(claveImagen);

                if (imagen == null)
                    return NotFound(new { mensaje = $"No se encontró la imagen con clave: {claveImagen}" });

                return Ok(imagen);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarImagen([FromForm] GaleriaImagenRequestDTO dto)
        {
            try
            {
                var imagenCreada = await _service.RegistrarImagenAsync(dto);
                return StatusCode(201, imagenCreada);
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

        [HttpPut("{claveImagen}")]
        public async Task<IActionResult> ActualizarImagen(string claveImagen, [FromForm] GaleriaImagenUpdateDTO dto)
        {
            try
            {
                await _service.ActualizarImagenAsync(claveImagen, dto);
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

        [HttpDelete("{claveImagen}")]
        public async Task<IActionResult> EliminarImagen(string claveImagen)
        {
            try
            {
                await _service.EliminarImagenAsync(claveImagen);
                return Ok(new { mensaje = $"Imagen {claveImagen} eliminada correctamente." });
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
                return BadRequest(new { mensaje = "No se pudo eliminar la imagen", detalle = ex.Message });
            }
        }
    }
}
