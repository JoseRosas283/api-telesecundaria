using Telesecundaria.DTOs.PermisosGestion.Request;
using Telesecundaria.DTOs.PermisosGestion.Response;
using Telesecundaria.Repositories.Implementations;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class PermisoService : IPermisoService
    {
        private readonly IPermisoRepository _repository;

        public PermisoService(IPermisoRepository repository)
        {
            _repository = repository;
        }

        public async Task<PermisoGestionarResponse> GestionarPermisosAsync(PermisoGestionarRequest request)
        {

            return await _repository.GestionarPermisosAsync(request);
        }
   }
}
