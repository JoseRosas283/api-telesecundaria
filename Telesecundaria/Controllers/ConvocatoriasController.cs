using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Convocatorias;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConvocatoriasController : ControllerBase
    {
        private readonly IConvocatoriasService _service;

        public ConvocatoriasController(IConvocatoriasService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarConvocatorias()
        {
            var convocatorias = await _service.ListarConvocatoriasAsync();

            if (!convocatorias.Any())
                return Ok(new { mensaje = "No hay convocatorias registradas." });

            return Ok(convocatorias);
        }

        [HttpGet("{claveConvocatoria}")]
        public async Task<IActionResult> ObtenerPorId(string claveConvocatoria)
        {
            try
            {
                var convocatoria = await _service.ObtenerPorIdAsync(claveConvocatoria);

                if (convocatoria == null)
                    return NotFound(new { mensaje = $"No se encontró la convocatoria con clave: {claveConvocatoria}" });

                return Ok(convocatoria);
            } 
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });    
            } 
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarConvocatoria([FromBody] ConvocatoriaRequestDTO dto)
        {
            try
            {
                var convocatoriaCreada = await _service.RegistrarConvocatoriaAsync(dto);
                return StatusCode(201, convocatoriaCreada);
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

        [HttpPut("{claveConvocatoria}")]
        public async Task<IActionResult> ActualizarConvocatoria(string claveConvocatoria, [FromBody] ConvocatoriaUpdateDTO dto)
        {
            try
            {
                await _service.ActualizarConvocatoriaAsync(claveConvocatoria, dto);
                return Ok(new
                {
                    mensaje = "Éxito: La convocatoria y su publicación web se han actualizado correctamente.",
                    clave = claveConvocatoria
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = "Error de validación", detalle = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = "No encontrado", detalle = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        [HttpDelete("{claveConvocatoria}")]
        public async Task<IActionResult> EliminarConvocatoria(string claveConvocatoria, [FromQuery] string nombreUsuario)
        {
            try
            {
                await _service.EliminarConvocatoriaAsync(claveConvocatoria, nombreUsuario);
                return Ok(new { mensaje = $"Convocatoria {claveConvocatoria} eliminada correctamente." });
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
                return BadRequest(new { mensaje = "No se pudo eliminar la convocatoria", detalle = ex.Message });
            }
        }
    }
}
