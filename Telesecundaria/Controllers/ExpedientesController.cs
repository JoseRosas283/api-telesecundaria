using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Expedientes.Request;
using Telesecundaria.Models;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [Authorize(Roles = "Administrativo,Directivo")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ExpedientesController : ControllerBase
    {
        private readonly IExpedienteService _service;

        public ExpedientesController(IExpedienteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var expedientes = await _service.GetAllAsync();
            return Ok(expedientes ?? new List<ExpedientesEntity>());
        }

        [HttpGet("ConsultarExpediente/{claveExpediente}")]
        public async Task<IActionResult> GetById(string claveExpediente)
        {
            var expediente = await _service.GetByIdAsync(claveExpediente);

            if (expediente == null)
                return NotFound(new { Mensaje = $"No existe el expediente con clave{claveExpediente}" });

            return Ok(expediente);
        }


        [HttpPost]
        public async Task<IActionResult> create([FromBody] ExpedienteCreateRequest request)
        {
            try
            {
                var nuevoExpediente = await _service.CreateAsync(request);

                return CreatedAtAction(nameof(GetById),
                    new { claveExpediente = nuevoExpediente.ClaveExpediente }, nuevoExpediente);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mesaje = ex.Message });
            }

            catch (InvalidOperationException ex)
            {

                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno: {ex.Message}" });

            }
        }

        [HttpPut("actualizarExpediente/{claveExpediente}")]

        public async Task<IActionResult> Update(string claveExpediente, [FromBody] ExpedienteUpdateRequest request)
        {
            try
            {
                await _service.UpdateAsync(claveExpediente, request);

                var actualizado = await _service.GetByIdAsync(claveExpediente);
                return Ok(actualizado);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mesaje = $"Error interno:{ex.Message}" });
            }

        }

        [HttpDelete("eliminarExpediente/{claveExpediente}")]
        public async Task<IActionResult> Delete(string claveExpediente)
        {
            try
            {
                await _service.DeleteAsync(claveExpediente);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Mensaje = ex.Message });
            }
            catch (InvalidOperationException ex)
            {

                return Conflict(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno: {ex.Message}" });
            }

        }

    }
}
