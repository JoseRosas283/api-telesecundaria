using Telesecundaria.DTOs.PermisosGestion.Request;
using Telesecundaria.DTOs.PermisosGestion.Response;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IPermisoRepository
    {
     public Task<PermisoGestionarResponse> GestionarPermisosAsync(PermisoGestionarRequest request);

    }
}
