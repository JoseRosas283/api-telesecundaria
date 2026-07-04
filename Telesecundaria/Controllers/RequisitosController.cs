using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Requisitos;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequisitosController : ControllerBase
    {
        private readonly IRequisitosService _service;

        public RequisitosController(IRequisitosService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarRequisitos()
        {
            var requisitos = await _service.ListarRequisitosAsync();
            if (!requisitos.Any())
                return Ok(new { mensaje = "No hay requisitos registrados." });
            return Ok(requisitos);
        }

        [HttpGet("{claveRequisito}")]
        public async Task<IActionResult> ObtenerPorId(string claveRequisito)
        {
            try
            {
                var requisito = await _service.ObtenerPorIdAsync(claveRequisito);
                if (requisito == null)
                    return NotFound(new { mensaje = $"No se encontró el requisito con clave: {claveRequisito}" });
                return Ok(requisito);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfigurarRequisitoEtapa([FromBody] RequisitoRequestDTO dto)
        {
            try
            {
                var insertado = await _service.ConfigurarRequisitoEtapaAsync(dto);
                return StatusCode(201, insertado);
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
