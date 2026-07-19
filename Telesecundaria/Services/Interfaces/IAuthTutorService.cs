using Telesecundaria.DTOs.AuthTutor.Request;
using Telesecundaria.DTOs.AuthTutor.Response;

namespace Telesecundaria.Services.Interfaces
{
    public interface IAuthTutorService
    {
        Task<LoginTutorResponse> LoginAsync(LoginTutorRequest request, string ip, string userAgent);

        Task<LoginTutorResponse> RefreshTokenAsync(string refreshToken);
        Task<LoginTutorResponse> LogoutAsync(string claveToken, string tokenOriginal);

        Task<RecuperacionResponse> SolicitarCodigoAsync(SolicitarCodigoRequest request);

        Task<ValidarCodigoResponse> ValidarCodigoAsync(ValidarCodigoRequest request);

        Task<RecuperacionResponse> ConfirmarCambioContrasenaAsync(ConfirmarCambioContrasenaRequest request);

      
    }
}
