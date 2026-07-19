using Telesecundaria.DTOs.Auth.Request;
using Telesecundaria.DTOs.Auth.Response;

namespace Telesecundaria.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request, string ip, string userAgent);
        Task<LoginResponse> LogoutAsync(string claveLogueo);

        Task<LoginResponse> RefreshTokenAsync(string refreshToken);
    }
}
