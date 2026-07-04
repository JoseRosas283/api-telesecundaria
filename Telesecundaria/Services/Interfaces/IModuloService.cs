using Telesecundaria.DTOs.Modulo.Request;
using Telesecundaria.DTOs.Modulo.Response;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IModuloService
    {
        Task<IEnumerable<ModuloResponse>> GetAllAsync();
        Task<ModuloResponse?> GetByIdAsync(string claveModulo);
        Task<ModuloResponse?> GetByNombreAsync(string nombreModulo);
        Task<ModuloResponse> CreateAsync(ModuloCreateRequest request);
    }
}
