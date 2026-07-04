using Telesecundaria.DTOs.RevisionesAceptadas;

namespace Telesecundaria.Services.Interfaces
{
    public interface IRevisionesAceptadasService
    {
        Task<IEnumerable<RevisionAceptadaResponseDTO>> ListarPendientesAsync();
        Task<RevisionAceptadaResponseDTO?> ObtenerPorIdAsync(string claveRevision);
        Task RegistrarAceptacionAsync(RevisionAceptadaRequestDTO dto);
    }
}
