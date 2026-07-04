using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.TipoDocumentos;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoDocumentosController : ControllerBase
    {
        private readonly ITipoDocumentosService _service;

        public TipoDocumentosController(ITipoDocumentosService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTipoDocumentos()
        {
            var documentos = await _service.ListarTipoDocumentosAsync();
            if (!documentos.Any())
                return Ok(new { mensaje = "No hay tipos de documento registrados." });
            return Ok(documentos);
        }

        [HttpGet("{clave}")]
        public async Task<IActionResult> ObtenerPorId(string clave)
        {
            try
            {
                var documento = await _service.ObtenerPorIdAsync(clave);
                if (documento == null)
                    return NotFound(new { mensaje = $"No se encontró el tipo de documento con clave: {clave}" });
                return Ok(documento);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertarTipoDocumento([FromBody] TipoDocumentoRequestDTO dto)
        {
            try
            {
                var insertado = await _service.InsertarTipoDocumentoAsync(dto);
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
