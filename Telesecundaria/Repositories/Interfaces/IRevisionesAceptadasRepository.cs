using Telesecundaria.DTOs.RevisionesAceptadas;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IRevisionesAceptadasRepository
    {
        Task<IEnumerable<RevisionesAceptadasEntity>> ListarPendientesAsync();
        Task<RevisionesAceptadasEntity?> ObtenerPorIdAsync(string claveRevision);
        Task RegistrarAceptacionAsync(RevisionAceptadaRequestDTO dto);
    }
}
