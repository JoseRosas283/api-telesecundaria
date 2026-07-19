using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Documentos.Request;
using Telesecundaria.DTOs.DocumentosAlumnos.Request;
using Telesecundaria.Models;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [Authorize(Roles = "Administrativo,Directivo")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentoService _service;

        public DocumentosController(IDocumentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var documentos = await _service.GetAllAsync();
            return Ok(documentos ?? new List<DocumentosEntity>());
        }

        [HttpGet("ConsultarDocumento/{claveDocumento}")]
        public async Task<IActionResult> GetById(string claveDocumento)
        {
            var documento = await _service.GetByIdAsync(claveDocumento);
            if (documento == null)
                return NotFound(new { Mensaje = $"No existe el documento con clave {claveDocumento}" });
            return Ok(documento);
        }

        [HttpPost("SubirDocumento")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] DocumentoCreateRequest request)
        {
            try
            {
                var resultado = await _service.UploadAsync(request);
                return StatusCode(201, resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return Conflict(new { Mensaje = ex.InnerException?.Message ?? ex.Message });
            }
        }


        [HttpPost("alumno/documentos")]
        [Authorize]
        public async Task<IActionResult> UploadAlumno([FromForm] DocumentoAlumnoCreateRequest request)
        {
            try
            {
                var resultado = await _service.UploadAlumnosAsync(request);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { mensaje = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" }); }
        }

        [HttpGet("expediente/{claveExpediente}")]
        [Authorize]
        public async Task<IActionResult> GetByExpediente(string claveExpediente)
        {
            var documentos = await _service.GetByExpedienteAsync(claveExpediente);
            return Ok(documentos);
        }

        [HttpPost("SubirDocumentoIntendente")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadIntendente([FromForm] DocumentoIntendenteCreateRequest request)
        {
            try
            {
                var resultado = await _service.UploadIntendenteAsync(request);
                return StatusCode(201, resultado);
            }
            catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
            catch (Exception ex) { return Conflict(new { mensaje = ex.InnerException?.Message ?? ex.Message }); }
        }



    }
}
