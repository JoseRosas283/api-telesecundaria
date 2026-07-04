using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Telesecundaria.DTOs.AuthTutor.Request;
using Telesecundaria.DTOs.AuthTutor.Response;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class AuthTutorService : IAuthTutorService
    {
        private readonly IAuthTutorRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthTutorService(IAuthTutorRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<LoginTutorResponse> LoginAsync(LoginTutorRequest request, string ip, string userAgent)
        {
            if (string.IsNullOrWhiteSpace(request.Correo))
                throw new ArgumentException("El correo es obligatorio.");
            if (string.IsNullOrWhiteSpace(request.Contrasenia))
                throw new ArgumentException("La contraseña es obligatoria.");

            var (exito, mensaje, claveToken, tokenOriginal, nombreTutor) =
                await _repository.IniciarSesionAsync(request, ip, userAgent);

            if (!exito)
                throw new UnauthorizedAccessException(mensaje);

            var token = GenerarToken(claveToken, tokenOriginal, nombreTutor);

            return new LoginTutorResponse
            {
                Token = token,
                ClaveToken = claveToken,
                NombreTutor = nombreTutor,
                Mensaje = mensaje
            };
        }

        public async Task<LoginTutorResponse> LogoutAsync(string claveToken, string tokenOriginal)
        {
            if (string.IsNullOrWhiteSpace(claveToken))
                throw new ArgumentException("La clave del token es obligatoria.");
            if (string.IsNullOrWhiteSpace(tokenOriginal))
                throw new ArgumentException("El token original es obligatorio.");

            var (exito, mensaje) = await _repository.CerrarSesionAsync(claveToken, tokenOriginal);

            if (!exito)
                throw new InvalidOperationException(mensaje);

            return new LoginTutorResponse { Mensaje = mensaje };
        }

        private string GenerarToken(string claveToken, string tokenOriginal, string nombreTutor)
        {
            var key = _configuration["Jwt:Key"]!;
            var expiracion = int.Parse(_configuration["Jwt:ExpiracionMinutos"]!);

            var claims = new[]
            {
            new Claim("claveToken",    claveToken),
            new Claim("tokenOriginal", tokenOriginal),
            new Claim("nombreTutor",   nombreTutor),
            new Claim(ClaimTypes.Role, "Tutor")
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
