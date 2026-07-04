using Telesecundaria.DTOs.EmpleadoRol.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IEmpleadoRolService
    {
        Task<IEnumerable<EmpleadoRolEntity>> GetAllAsync();

        Task<EmpleadoRolEntity?> GetByIdAsync(string claveRol);

        Task<EmpleadoRolEntity> CreateAsync(EmpleadoRolCreateRequest request);
    }
}
