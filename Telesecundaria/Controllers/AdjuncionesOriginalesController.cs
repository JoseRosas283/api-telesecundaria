using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.AdjuncionesOriginales;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdjuncionesOriginalesController : ControllerBase
    {
        private readonly IAdjuncionesOriginalesService _service;

        public AdjuncionesOriginalesController(IAdjuncionesOriginalesService service)
        {
            _service = service;
        }

        // Abre el expediente digital original (1 vez por entrega)
        [HttpPost]
        public async Task<IActionResult> RegistrarAdjuncionOriginal([FromBody] AdjuncionOriginalRequestDTO dto)
        {
            try
            {
                var resultado = await _service.RegistrarAdjuncionOriginalAsync(dto);
                return StatusCode(201, resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", detalle = ex.Message });
            }
        }

        // Registra cada documento físico cotejado (se llama N veces, uno por documento)
        [HttpPost("detalle")]
        public async Task<IActionResult> RegistrarDetalle([FromBody] DetalleAdjuncionOriginalRequestDTO dto)
        {
            try
            {
                var resultado = await _service.RegistrarDetalleAsync(dto);
                return StatusCode(201, resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", detalle = ex.Message });
            }
        }

        // Consulta cuántos documentos van vs cuántos se requieren (NO cierra nada, solo informa)
        [HttpGet("{claveAdjOriginal}/progreso")]
        public async Task<IActionResult> ObtenerProgreso(string claveAdjOriginal)
        {
            try
            {
                var resultado = await _service.ObtenerProgresoAsync(claveAdjOriginal);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", detalle = ex.Message });
            }
        }
    }
}
