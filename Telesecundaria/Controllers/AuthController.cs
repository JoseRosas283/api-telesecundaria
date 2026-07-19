using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telesecundaria.DTOs.Auth.Request;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                var userAgent = Request.Headers["User-Agent"].ToString();
                var response = await _service.LoginAsync(request, ip, userAgent);

                SetRefreshTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        // NUEVO
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    return Unauthorized(new { mensaje = "No se encontró refresh token." });

                var response = await _service.RefreshTokenAsync(refreshToken);

                SetRefreshTokenCookie(response.RefreshToken);

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var claveLogueo = User.FindFirst("claveLogueo")?.Value;
                if (string.IsNullOrWhiteSpace(claveLogueo))
                    return Unauthorized(new { mensaje = "No se encontró sesión activa en el token." });

                var response = await _service.LogoutAsync(claveLogueo);

                Response.Cookies.Delete("refreshToken");

                return Ok(new { mensaje = response.Mensaje });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error interno: {ex.Message}" });
            }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var diasRefresh = int.Parse(_configuration["Jwt:RefreshTokenExpiracionDias"] ?? "7");

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, //  cámbialo a true en producción (requiere HTTPS)
                SameSite = SameSiteMode.Lax, // en local con http, Strict a veces bloquea la cookie
                Expires = DateTimeOffset.UtcNow.AddDays(diasRefresh)
            });
        }
    }
}