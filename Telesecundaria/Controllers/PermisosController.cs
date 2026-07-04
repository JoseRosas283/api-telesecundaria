using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.PermisosGestion.Request;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [Authorize(Roles = "Administrativo,Directivo")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PermisosController : ControllerBase
    {

        private readonly IPermisoService _service;

        public PermisosController(IPermisoService service)
        {
            _service = service;
        }

        [HttpPost("gestionar")]
        public async Task<IActionResult> GestionarPermisos([FromBody] PermisoGestionarRequest request)
        {
            try
            {
                var response = await _service.GestionarPermisosAsync(request);
                if (response.Exito)
                    return Ok(response);
                else
                    return BadRequest(new { Mensaje = response.Mensaje });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensaje = $"Error interno: {ex.Message}" });
            }
        }

    }
}
