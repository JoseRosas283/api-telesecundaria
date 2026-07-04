using Telesecundaria.DTOs.Empleados.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IEmpleadosRepository
    {
        Task<IEnumerable<EmpleadosEntity>> GetAllAsync();

        Task<EmpleadosEntity> GetByIdAsync(string claveEmpleado);

        Task<EmpleadosEntity> CreateAsync(EmpleadoCreateRequest request);

        Task UpdateAsync(string claveEmpleado, EmpleadoUpdateRequest request);

        Task DeleteAsync(string claveEmpleado);

    }
}
