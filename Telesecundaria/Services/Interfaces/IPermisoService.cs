using Telesecundaria.DTOs.PermisosGestion.Request;
using Telesecundaria.DTOs.PermisosGestion.Response;

namespace Telesecundaria.Services.Interfaces
{
    public interface IPermisoService
    {
        Task<PermisoGestionarResponse> GestionarPermisosAsync(PermisoGestionarRequest request);
    }
}
