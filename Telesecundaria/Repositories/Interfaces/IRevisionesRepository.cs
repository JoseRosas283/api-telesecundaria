using Telesecundaria.DTOs.Revisiones;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IRevisionesRepository
    {
        Task<IEnumerable<RevisionesEntity>> ListarRevisionesAsync();
        Task<RevisionesEntity?> ObtenerPorIdAsync(string clave);
        Task ProcesarRevisionAsync(RevisionRequestDTO dto);
        Task<RevisionesEntity?> ObtenerUltimaPorUsuarioAsync(string claveUsuario);
        Task CerrarRevisionAsync(string claveRevision);
    }
}
