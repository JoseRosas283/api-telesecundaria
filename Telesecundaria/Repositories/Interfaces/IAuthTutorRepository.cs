

using Telesecundaria.DTOs.AuthTutor.Request;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IAuthTutorRepository
    {
        Task<(bool exito, string mensaje, string claveToken, string tokenOriginal, string nombreTutor, string claveTutorAspirante)> IniciarSesionAsync(LoginTutorRequest request, string ip, string userAgent);

        Task<(bool exito, string mensaje)> CerrarSesionAsync(string claveToken, string tokenOriginal);
    }
}
