

using Telesecundaria.DTOs.Auth.Request;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<(bool exito, string mensaje, string claveUsuario)> IniciarSesionAsync(LoginRequest request, string ip, string userAgent);

        Task<string> ObtenerClaveLogueoAsync(string claveUsuario);

        Task<string> ObtenerRolAsync(string claveUsuario);

        Task<(bool exito, string mensaje)> CerrarSesionAsync(string claveLogueo);
    }
}
