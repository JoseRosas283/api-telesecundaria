using Telesecundaria.DTOs.EmpleadoRol.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IEmpleadoRolRepository
    {
        Task<IEnumerable<EmpleadoRolEntity>> GetAllAsync();

        Task<EmpleadoRolEntity> GetByIdAsync(string claveRol);

        Task<EmpleadoRolEntity> CreateAsync(EmpleadoRolCreateRequest request);
    }
}
