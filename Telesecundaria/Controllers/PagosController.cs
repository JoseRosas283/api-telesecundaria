using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Pagos;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagosController : ControllerBase
    {
        private readonly IPagosService _service;

        public PagosController(IPagosService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarPagos()
        {
            var pagos = await _service.ListarPagosAsync();
            if (!pagos.Any())
                return Ok(new { mensaje = "No hay pagos registrados." });
            return Ok(pagos);
        }

        [HttpGet("{clavePago}")]
        public async Task<IActionResult> ObtenerPorId(string clavePago)
        {
            try
            {
                var pago = await _service.ObtenerPorIdAsync(clavePago);
                if (pago == null)
                    return NotFound(new { mensaje = $"No se encontró el pago con clave: {clavePago}" });
                return Ok(pago);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPago([FromBody] PagoRequestDTO dto)
        {
            try
            {
                await _service.RegistrarPagoAsync(dto);
                return StatusCode(201, new { mensaje = "Pago registrado correctamente." });
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
    }
}
