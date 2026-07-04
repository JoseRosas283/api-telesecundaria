using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Aspirantes;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AspirantesController : ControllerBase
    {
        private readonly IAspirantesService _service;

        public AspirantesController(IAspirantesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarAspirantes()
        {
            var aspirantes = await _service.ListarAspirantesAsync();

            if (!aspirantes.Any())
                return Ok(new { mensaje = "No hay aspirantes registrados." });

            return Ok(aspirantes);
        }

        [HttpGet("{claveAspirante}")]
        public async Task<IActionResult> ObtenerPorId(string claveAspirante)
        {
            var aspirante = await _service.ObtenerPorIdAsync(claveAspirante);

            if (aspirante == null)
                return NotFound(new { mensaje = $"No se encontró el aspirante con clave: {claveAspirante}" });

            return Ok(aspirante);
        }

        [HttpGet("curp/{curp}")]
        public async Task<IActionResult> ObtenerPorCurp(string curp)
        {
            var aspirante = await _service.ObtenerPorCurpAsync(curp);

            if (aspirante == null)
                return NotFound(new { mensaje = $"No se encontró ningún aspirante con CURP: {curp}" });

            return Ok(aspirante);
        }

        [HttpGet("porConvocatoria/{claveConvocatoria}")]
        public async Task<IActionResult> ObtenerPorConvocatoria(string claveConvocatoria)
        {
            var aspirantes = await _service.ObtenerPorConvocatoriaAsync(claveConvocatoria);

            if (!aspirantes.Any())
                return Ok(new { mensaje = "No hay aspirantes registrados en esta convocatoria." });

            return Ok(aspirantes);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarAspirante([FromBody] AspiranteRequestDTO dto)
        {
            try
            {
                var aspiranteCreado = await _service.RegistrarAspiranteAsync(dto);
                return StatusCode(201, aspiranteCreado);
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

        [HttpPut("{claveAspirante}")]
        public async Task<IActionResult> ActualizarAspirante(string claveAspirante, [FromBody] AspiranteUpdateDTO dto)
        {
            try
            {
                await _service.ActualizarAspiranteAsync(claveAspirante, dto);
                return Ok(new
                {
                    mensaje = "Éxito: El registro del aspirante se ha actualizado correctamente.",
                    clave = claveAspirante
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

        [HttpDelete("{claveAspirante}")]
        public async Task<IActionResult> EliminarAspirante(string claveAspirante)
        {
            try
            {
                await _service.EliminarAspiranteAsync(claveAspirante);
                return Ok(new { mensaje = $"Aspirante {claveAspirante} eliminado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = "Error en el parámetro", detalle = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = "No encontrado", detalle = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "No se pudo eliminar el aspirante", detalle = ex.Message });
            }
        }
    }
}
