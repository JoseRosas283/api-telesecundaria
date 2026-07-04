using Telesecundaria.DTOs.DestinoNotificacion;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IDestinoNotificacionRepository
    {
        Task<IEnumerable<DestinoNotificacionEntity>> ListarAsync();
        Task<DestinoNotificacionEntity?> ObtenerPorIdAsync(string clave);
        Task RegistrarAsync(DestinoNotificacionRequestDTO dto);
        Task<DestinoNotificacionEntity?> ObtenerUltimoPorProcesoYReceptorAsync(string nombreProceso, string tipoReceptor);
    }
}
