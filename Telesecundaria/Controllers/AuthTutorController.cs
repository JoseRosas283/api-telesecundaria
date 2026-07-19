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

        private readonly IConfiguration _configuration;

        public AuthTutorController(IAuthTutorService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
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
                SetRefreshTokenCookie(resultado.RefreshToken);
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

                Response.Cookies.Delete("refreshTokenTutor");
                return Ok(resultado);
            }
            catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { mensaje = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" }); }
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshTokenTutor"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new { mensaje = "No se encontró refresh token." });

                var resultado = await _service.RefreshTokenAsync(refreshToken);

                SetRefreshTokenCookie(resultado.RefreshToken);

                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { mensaje = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" }); }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var diasRefresh = int.Parse(_configuration["Jwt:RefreshTokenExpiracionDias"] ?? "7");
            Response.Cookies.Append("refreshTokenTutor", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,              // mismo criterio que Admin: cámbialo a true en producción
                SameSite = SameSiteMode.Lax, // mismo criterio que Admin
                Expires = DateTimeOffset.UtcNow.AddDays(diasRefresh)
            });
        }


        [HttpPost("test-correo")]
        [AllowAnonymous]
        public async Task<IActionResult> TestCorreo([FromServices] IEmailService emailService)
        {
            var ok = await emailService.EnviarCorreoAsync(
                "alexdjhernandew@gmail.com",
                "Prueba del sistema",
                "Si ves este mensaje, el correo funciona correctamente."
            );
            return Ok(new { enviado = ok });
        }

        [HttpPost("solicitar-codigo")]
        [AllowAnonymous]
        public async Task<IActionResult> SolicitarCodigo([FromBody] SolicitarCodigoRequest request)
        {
            try
            {
                var resultado = await _service.SolicitarCodigoAsync(request);
                return Ok(resultado);
            }
            catch (Exception ex) { return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" }); }
        }

        [HttpPost("validar-codigo")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidarCodigo([FromBody] ValidarCodigoRequest request)
        {
            try
            {
                var resultado = await _service.ValidarCodigoAsync(request);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { mensaje = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" }); }
        }

        [HttpPost("confirmar-cambio")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmarCambio([FromBody] ConfirmarCambioContrasenaRequest request)
        {
            try
            {
                var resultado = await _service.ConfirmarCambioContrasenaAsync(request);
                return Ok(resultado);
            }
            catch (ArgumentException ex) { return BadRequest(new { mensaje = ex.Message }); }
            catch (UnauthorizedAccessException ex) { return Unauthorized(new { mensaje = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" }); }
        }


    }
}
