using Telesecundaria.DTOs.Roles.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IRolesRepository
    {
        Task<IEnumerable<RolesEntity>> GetAllAsync();

        Task<RolesEntity> GetByIdAsync(string claveRol);


        Task<RolesEntity> CreateAsync(RolesCreateRequest request);
    }
}
