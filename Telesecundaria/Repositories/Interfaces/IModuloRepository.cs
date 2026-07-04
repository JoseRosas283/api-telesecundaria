using Telesecundaria.DTOs.Modulo.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IModuloRepository
    {
        Task<IEnumerable<ModulosEntity>> GetAllAsync();
        Task<ModulosEntity?> GetByIdAsync(string claveModulo);
        Task<ModulosEntity?> GetByNombreAsync(string nombreModulo);
        Task<ModulosEntity> CreateAsync(ModuloCreateRequest request);

    }
}
