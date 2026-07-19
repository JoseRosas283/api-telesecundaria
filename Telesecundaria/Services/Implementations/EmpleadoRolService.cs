using Telesecundaria.DTOs.EmpleadoRol.Request;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class EmpleadoRolService : IEmpleadoRolService
    {
        private readonly IEmpleadoRolRepository _repository;

        public EmpleadoRolService(IEmpleadoRolRepository repository)
        {

            _repository = repository;

        }


        public async Task<IEnumerable<EmpleadoRolEntity>> GetAllAsync()
        {
            var rol = await _repository.GetAllAsync();
            return rol;
        }

        public async Task<EmpleadoRolEntity?> GetByIdAsync(string claveRol)
        {
            if (string.IsNullOrWhiteSpace(claveRol))
                throw new ArgumentException("La clave rol es obligatorio");

            var rol = await _repository.GetByIdAsync(claveRol);
            return rol;
        }


        public async Task<EmpleadoRolEntity> CreateAsync(RolCreateRequest request)
        {
            
            return await _repository.CreateAsync(request);
        }
    }
}
