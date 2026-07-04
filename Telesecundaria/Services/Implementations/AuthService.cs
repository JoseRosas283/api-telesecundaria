using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Telesecundaria.DTOs.Auth.Request;
using Telesecundaria.DTOs.Auth.Response;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, string ip, string userAgent)
        {
            if (string.IsNullOrWhiteSpace(request.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(request.Contrasenia))
                throw new ArgumentException("La contraseña es obligatoria.");

            var (exito, mensaje, claveUsuario) = await _repository.IniciarSesionAsync(request, ip, userAgent);

            if (!exito)
                throw new UnauthorizedAccessException(mensaje);

            var claveLogueo = await _repository.ObtenerClaveLogueoAsync(claveUsuario);
            var rol = await _repository.ObtenerRolAsync(claveUsuario);
            var token = GenerarToken(claveUsuario, claveLogueo, rol);

            return new LoginResponse
            {
                Token = token,
                ClaveUsuario = claveUsuario,
                ClaveLogueo = claveLogueo,
                Rol = rol,
                Mensaje = mensaje
            };
        }

        public async Task<LoginResponse> LogoutAsync(string claveLogueo)
        {
            if (string.IsNullOrWhiteSpace(claveLogueo))
                throw new ArgumentException("La clave de logueo es obligatoria.");

            var (exito, mensaje) = await _repository.CerrarSesionAsync(claveLogueo);

            if (!exito)
                throw new InvalidOperationException(mensaje);

            return new LoginResponse
            {
                Mensaje = mensaje
            };
        }

        private string GenerarToken(string claveUsuario, string claveLogueo, string rol)
        {
            var key = _configuration["Jwt:Key"]!;
            var expiracion = int.Parse(_configuration["Jwt:ExpiracionMinutos"]!);

            var claims = new[]
            {
                new Claim("claveUsuario", claveUsuario),
                new Claim("claveLogueo", claveLogueo),
                new Claim(ClaimTypes.Role, rol)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiracion),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
