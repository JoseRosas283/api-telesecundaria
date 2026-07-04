using Telesecundaria.DTOs.Roles.Request;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class RolesService : IRolesService
    {
        private readonly IRolesRepository _repository;

        public RolesService(IRolesRepository repository)
        {
            _repository = repository;
        }


        public async Task<IEnumerable<RolesEntity>> GetAllAsync()
        {
            var roles = await _repository.GetAllAsync();
            return roles;
        }

        public async Task<RolesEntity?> GetByIdAsync(string claveRol)
        {
            if (string.IsNullOrWhiteSpace(claveRol))
                throw new ArgumentNullException("La clave no puede estar vacia");

            var roles = await _repository.GetByIdAsync(claveRol);
            return roles;

        }


        public async Task<RolesEntity> CreateAsync(RolesCreateRequest request)
        {

            if (string.IsNullOrWhiteSpace(request.NombreRol))
                throw new ArgumentNullException("Debe tener un nombre el rol");

            if (string.IsNullOrWhiteSpace(request.Descripcion))
                throw new ArgumentNullException("La descripcion debe ser obligatoria");

            return await _repository.CreateAsync(request);
        }
    }
}
