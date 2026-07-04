using Telesecundaria.DTOs.AuthTutor.Request;
using Telesecundaria.DTOs.AuthTutor.Response;

namespace Telesecundaria.Services.Interfaces
{
    public interface IAuthTutorService
    {
        Task<LoginTutorResponse> LoginAsync(LoginTutorRequest request, string ip, string userAgent);
        Task<LoginTutorResponse> LogoutAsync(string claveToken, string tokenOriginal);
    }
}
