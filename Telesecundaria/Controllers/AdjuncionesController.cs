using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Adjunciones;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdjuncionesController : ControllerBase
    {
        private readonly IAdjuncionesService _service;

        public AdjuncionesController(IAdjuncionesService service)
        {
            _service = service;
        }

        // Recibe los 5 documentos en un solo POST
        [HttpPost("registrar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegistrarAdjuncion([FromForm] AdjuncionRequestDTO dto)
        {
            try
            {
                var resultado = await _service.RegistrarAdjuncionAsync(dto);
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

        // Carga de un documento individual
        [HttpPost("documentos/temp")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegistrarDocumentoTemp([FromForm] DocumentoTempRequestDTO dto)
        {
            try
            {
                var resultado = await _service.RegistrarDocumentoTempAsync(dto);
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

        // Consulta de estado de documentos cargados
        [HttpGet("documentos/estado/{claveAspirante}")]
        public async Task<IActionResult> ObtenerEstadoDocumentos(string claveAspirante)
        {
            try
            {
                var resultado = await _service.ObtenerEstadoDocumentosAsync(claveAspirante);
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

        // Finaliza creando la adjunción y vinculando documentos ya cargados
        [HttpPost("finalizar")]
        public async Task<IActionResult> FinalizarAdjuncion([FromBody] FinalizarAdjuncionRequestDTO dto)
        {
            try
            {
                var resultado = await _service.FinalizarAdjuncionAsync(dto);
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
    }
}
