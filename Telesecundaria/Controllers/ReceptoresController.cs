using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Receptores;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceptoresController : ControllerBase
    {
        private readonly IReceptoresService _service;

        public ReceptoresController(IReceptoresService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var receptores = await _service.ListarAsync();
            if (!receptores.Any())
                return Ok(new { mensaje = "No hay receptores registrados." });
            return Ok(receptores);
        }

        [HttpGet("{claveReceptor}")]
        public async Task<IActionResult> ObtenerPorId(string claveReceptor)
        {
            try
            {
                var receptor = await _service.ObtenerPorIdAsync(claveReceptor);
                if (receptor == null)
                    return NotFound(new { mensaje = $"No se encontró el receptor con clave: {claveReceptor}" });
                return Ok(receptor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] ReceptorRequestDTO dto)
        {
            try
            {
                var creado = await _service.RegistrarAsync(dto);
                return StatusCode(201, creado);
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

        [HttpPut("{claveReceptor}")]
        public async Task<IActionResult> Actualizar(string claveReceptor, [FromBody] ReceptorUpdateDTO dto)
        {
            try
            {
                var actualizado = await _service.ActualizarAsync(claveReceptor, dto);
                return Ok(actualizado);
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
                return StatusCode(500, new { mensaje = "Error interno", detalle = ex.Message });
            }
        }

        [HttpDelete("{claveReceptor}")]
        public async Task<IActionResult> Eliminar(string claveReceptor)
        {
            try
            {
                await _service.EliminarAsync(claveReceptor);
                return Ok(new { mensaje = $"Receptor {claveReceptor} eliminado o desactivado correctamente." });
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
                return BadRequest(new { mensaje = "No se pudo eliminar el receptor", detalle = ex.Message });
            }
        }
    }
}
