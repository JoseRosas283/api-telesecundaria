using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.AuthTutor.Request;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/v1/Auth/Tutor")]
    public class AuthTutorController : ControllerBase
    {
        private readonly IAuthTutorService _service;

        public AuthTutorController(IAuthTutorService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginTutorRequest request)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                var userAgent = Request.Headers["User-Agent"].ToString();
                var resultado = await _service.LoginAsync(request, ip, userAgent);
                return Ok(resultado);
            }
            catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { mensaje = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" }); }
        }

        [HttpPost("logout")]
        [Authorize(Roles = "Tutor")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var claveToken = User.FindFirst("claveToken")?.Value
                    ?? throw new ArgumentException("No se encontró claveToken en el token.");
                var tokenOriginal = User.FindFirst("tokenOriginal")?.Value
                    ?? throw new ArgumentException("No se encontró tokenOriginal en el token.");
                var claveTutorAspirante = User.FindFirst("claveTutorAspirante")?.Value ?? string.Empty;

                var resultado = await _service.LogoutAsync(claveToken, tokenOriginal);
                resultado.ClaveTutorAspirante = claveTutorAspirante;
                return Ok(resultado);
            }
            catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" }); }
        }
    }
}
