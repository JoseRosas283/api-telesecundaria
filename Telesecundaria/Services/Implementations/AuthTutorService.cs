using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        private readonly IEmailService _emailService;
        private const string HashDummy = "$2a$11$CwTycUXWue0Thq9StjUM0uJ8i6ZzO/9OyoBWQ9zqz.dgw3fQ0v3wS";

        public AuthTutorService(IAuthTutorRepository repository, IConfiguration configuration, IEmailService emailService)
        {
            _repository = repository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<LoginTutorResponse> LoginAsync(LoginTutorRequest request, string ip, string userAgent)
        {
            var correo = request.Correo.Trim();
            var credenciales = await _repository.ObtenerCredencialesAsync(correo);

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

            var requestParaSp = new LoginTutorRequest
            {
                Correo = correo,
                Contrasenia = valorParaElSp
            };

            var (exito, mensaje, claveToken, tokenOriginal, nombreTutor, claveTutorAspirante) =
                await _repository.IniciarSesionAsync(requestParaSp, ip, userAgent);

            if (!exito)
                throw new UnauthorizedAccessException(mensaje);

            var token = GenerarToken(claveToken, tokenOriginal, nombreTutor, claveTutorAspirante);
            var refreshToken = GenerarRefreshToken();
            var diasRefresh = int.Parse(_configuration["Jwt:RefreshTokenExpiracionDias"] ?? "7");
            await _repository.GuardarRefreshTokenAsync(claveTutorAspirante, claveToken, refreshToken, DateTime.UtcNow.AddDays(diasRefresh));

            return new LoginTutorResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ClaveToken = claveToken,
                ClaveTutorAspirante = claveTutorAspirante,
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

        private string GenerarToken(string claveToken, string tokenOriginal, string nombreTutor, string claveTutorAspirante)
        {
            var key = _configuration["Jwt:Key"]!;
            var expiracion = int.Parse(_configuration["Jwt:ExpiracionMinutos"]!);

            var claims = new[]
            {
            new Claim("claveToken",    claveToken),
            new Claim("tokenOriginal", tokenOriginal),
            new Claim("nombreTutor",   nombreTutor),
            new Claim("claveTutorAspirante", claveTutorAspirante),
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

        public async Task<LoginTutorResponse> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedAccessException("Refresh token no proporcionado.");

            var datos = await _repository.ValidarRefreshTokenAsync(refreshToken);
            if (datos == null)
                throw new UnauthorizedAccessException("Refresh token inválido o expirado.");

            // Revocamos el usado (rotación, igual que en AuthService)
            await _repository.RevocarRefreshTokenAsync(refreshToken);

            var nuevoAccessToken = GenerarToken(datos.ClaveToken, tokenOriginal: string.Empty,
                nombreTutor: string.Empty, claveTutorAspirante: datos.ClaveTutorAspirante);

            var nuevoRefreshToken = GenerarRefreshToken();
            var diasRefresh = int.Parse(_configuration["Jwt:RefreshTokenExpiracionDias"] ?? "7");
            await _repository.GuardarRefreshTokenAsync(datos.ClaveTutorAspirante, datos.ClaveToken, nuevoRefreshToken, DateTime.UtcNow.AddDays(diasRefresh));

            return new LoginTutorResponse
            {
                Token = nuevoAccessToken,
                RefreshToken = nuevoRefreshToken,
                ClaveToken = datos.ClaveToken,
                ClaveTutorAspirante = datos.ClaveTutorAspirante,
                Mensaje = "Token renovado correctamente."
            };
        }


        public async Task<RecuperacionResponse> SolicitarCodigoAsync(SolicitarCodigoRequest request)
        {
            var (exito, mensaje, claveTutorAspirante, codigo) =
                await _repository.GenerarCodigoRecuperacionAsync(request.Correo);

            if (exito && !string.IsNullOrEmpty(codigo))
            {
                await _emailService.EnviarCorreoAsync(
                    request.Correo,
                    "Código de recuperación de contraseña",
                    $"Tu código de verificación es: {codigo}. Expira en 20 segundos."
                );
            }

            // Mismo mensaje exista o no el correo, para no filtrar cuentas registradas
            return new RecuperacionResponse { Mensaje = mensaje };
        }


        public async Task<ValidarCodigoResponse> ValidarCodigoAsync(ValidarCodigoRequest request)
        {
            var (exito, mensaje, token) =
                await _repository.ValidarCodigoRecuperacionAsync(request.Correo, request.Codigo);

            if (!exito)
                throw new UnauthorizedAccessException(mensaje);

            return new ValidarCodigoResponse { Mensaje = mensaje, TokenConfirmacion = token };
        }

        public async Task<RecuperacionResponse> ConfirmarCambioContrasenaAsync(ConfirmarCambioContrasenaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NuevaContrasena) || request.NuevaContrasena.Length < 8)
                throw new ArgumentException("La nueva contraseña debe tener al menos 8 caracteres.");

            var nuevaContrasenaHash = BCrypt.Net.BCrypt.HashPassword(request.NuevaContrasena);

            var (exito, mensaje) = await _repository.ConfirmarCambioContrasenaAsync(
                request.Correo, request.TokenConfirmacion, nuevaContrasenaHash);

            if (!exito)
                throw new UnauthorizedAccessException(mensaje);

            return new RecuperacionResponse { Mensaje = mensaje };
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
