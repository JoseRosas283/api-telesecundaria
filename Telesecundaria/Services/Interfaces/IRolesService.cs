using Telesecundaria.DTOs.Roles.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IRolesService
    {
        Task<IEnumerable<RolesEntity>> GetAllAsync();

        Task<RolesEntity?> GetByIdAsync(string claveRol);


        Task<RolesEntity> CreateAsync(RolesCreateRequest request);
    }
}
