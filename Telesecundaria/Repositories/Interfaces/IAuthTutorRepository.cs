

using Telesecundaria.DTOs.AuthTutor.Internal;
using Telesecundaria.DTOs.AuthTutor.Request;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IAuthTutorRepository
    {
        Task<(bool exito, string mensaje, string claveToken, string tokenOriginal, string nombreTutor, string claveTutorAspirante)> IniciarSesionAsync(LoginTutorRequest request, string ip, string userAgent);

        Task<(bool exito, string mensaje)> CerrarSesionAsync(string claveToken, string tokenOriginal);

        Task<TutorCredencialesDto?> ObtenerCredencialesAsync(string correo);

        Task GuardarRefreshTokenAsync(string claveTutorAspirante, string claveToken, string refreshToken, DateTime expiracion);

        Task<RefreshTokenTutor?> ValidarRefreshTokenAsync(string refreshToken);

        Task RevocarRefreshTokenAsync(string refreshToken);

        Task<(bool exito, string mensaje, string claveTutorAspirante, string codigo)> GenerarCodigoRecuperacionAsync(string correo);


        Task<(bool exito, string mensaje, string tokenConfirmacion)> ValidarCodigoRecuperacionAsync(string correo, string codigo);

        Task<(bool exito, string mensaje)> ConfirmarCambioContrasenaAsync(string correo, string tokenConfirmacion, string nuevaContrasenaHash);



    }
}
