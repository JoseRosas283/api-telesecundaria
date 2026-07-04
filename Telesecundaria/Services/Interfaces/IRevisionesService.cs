using Telesecundaria.DTOs.Revisiones;

namespace Telesecundaria.Services.Interfaces
{
    public interface IRevisionesService
    {
        Task<IEnumerable<RevisionResponseDTO>> ListarRevisionesAsync();
        Task<RevisionResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<RevisionResponseDTO> ProcesarRevisionAsync(RevisionRequestDTO dto);
        Task<RevisionResponseDTO> CerrarRevisionAsync(string claveRevision);
    }
}
