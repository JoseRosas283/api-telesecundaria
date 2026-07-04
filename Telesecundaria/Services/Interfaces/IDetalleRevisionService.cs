using Telesecundaria.DTOs.DetalleRevision;

namespace Telesecundaria.Services.Interfaces
{
    public interface IDetalleRevisionService
    {
        Task<IEnumerable<DetalleRevisionResponseDTO>> ListarPorRevisionAsync(string claveRevision);
        Task<DetalleRevisionResponseDTO?> ObtenerPorIdAsync(string claveRevision, string claveDocAspirante);
        Task RegistrarDetalleAsync(DetalleRevisionRequestDTO dto);
        Task<DetalleRevisionResponseDTO> ActualizarDetalleAsync(string claveRevision, string claveDocAspirante, DetalleRevisionUpdateDTO dto);
    }
}
