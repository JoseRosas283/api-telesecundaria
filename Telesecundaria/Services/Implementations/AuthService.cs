using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

        private const string HashDummy = "$2a$11$CwTycUXWue0Thq9StjUM0uJ8i6ZzO/9OyoBWQ9zqz.dgw3fQ0v3wS";

        public AuthService(IAuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, string ip, string userAgent)
        {
           
            var nombreUsuario = request.NombreUsuario.Trim();
            var credenciales = await _repository.obtenerCredencialesAsync(nombreUsuario);

            string valorParaElSp;

            if (credenciales != null)
            {
                bool passwordValido = BCrypt.Net.BCrypt.Verify(request.Contrasenia, credenciales.PasswordHash);

                valorParaElSp = passwordValido
                    ? credenciales.PasswordHash
                    : credenciales.PasswordHash + "_no_valido";
            }
            else
            {
                BCrypt.Net.BCrypt.Verify(request.Contrasenia, HashDummy);
                valorParaElSp = request.Contrasenia;
            }

            var requestParaSp = new LoginRequest
            {
                NombreUsuario = nombreUsuario,
                Contrasenia = valorParaElSp
            };

            var (exito, mensaje, claveUsuario) = await _repository.IniciarSesionAsync(requestParaSp, ip, userAgent);

            if (!exito)
                throw new UnauthorizedAccessException(mensaje);

            var claveLogueo = await _repository.ObtenerClaveLogueoAsync(claveUsuario);
            var rol = await _repository.ObtenerRolAsync(claveUsuario);
            var token = GenerarToken(claveUsuario, claveLogueo, rol);

            // NUEVO — generar y guardar el refresh token
            var refreshToken = GenerarRefreshToken();
            var diasRefresh = int.Parse(_configuration["Jwt:RefreshTokenExpiracionDias"] ?? "7");
            await _repository.GuardarRefreshTokenAsync(claveUsuario, claveLogueo, refreshToken, DateTime.UtcNow.AddDays(diasRefresh));

            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ClaveUsuario = claveUsuario,
                ClaveLogueo = claveLogueo,
                Rol = rol,
                Mensaje = mensaje
            };
        }

        // Refresh -Token
        public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedAccessException("Refresh token no proporcionado.");

            var datos = await _repository.ValidarRefreshTokenAsync(refreshToken);
            if (datos == null)
                throw new UnauthorizedAccessException("Refresh token inválido o expirado.");

            var rol = await _repository.ObtenerRolAsync(datos.ClaveUsuario);

           
            await _repository.RevocarRefreshTokenAsync(refreshToken);

            var nuevoAccessToken = GenerarToken(datos.ClaveUsuario, datos.ClaveLogueo, rol);
            var nuevoRefreshToken = GenerarRefreshToken();

            var diasRefresh = int.Parse(_configuration["Jwt:RefreshTokenExpiracionDias"] ?? "7");
            await _repository.GuardarRefreshTokenAsync(datos.ClaveUsuario, datos.ClaveLogueo, nuevoRefreshToken, DateTime.UtcNow.AddDays(diasRefresh));

            return new LoginResponse
            {
                Token = nuevoAccessToken,
                RefreshToken = nuevoRefreshToken,
                ClaveUsuario = datos.ClaveUsuario,
                ClaveLogueo = datos.ClaveLogueo,
                Rol = rol,
                Mensaje = "Token renovado correctamente."
            };
        }

        public async Task<LoginResponse> LogoutAsync(string claveLogueo)
        {
            if (string.IsNullOrWhiteSpace(claveLogueo))
                throw new ArgumentException("La clave de logueo es obligatoria.");

            var (exito, mensaje) = await _repository.CerrarSesionAsync(claveLogueo);

            if (!exito)
                throw new InvalidOperationException(mensaje);

            return new LoginResponse { Mensaje = mensaje };
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

     
        private string GenerarRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}