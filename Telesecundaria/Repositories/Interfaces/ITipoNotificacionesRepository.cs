using Telesecundaria.DTOs.TipoNotificaciones;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface ITipoNotificacionesRepository
    {
        Task<IEnumerable<TipoNotificacionesEntity>> ListarAsync();
        Task<TipoNotificacionesEntity?> ObtenerPorIdAsync(string clave);
        Task<TipoNotificacionesEntity?> ObtenerPorNombreProcesoAsync(string nombreProceso);
        Task RegistrarAsync(TipoNotificacionRequestDTO dto);
    }
}
