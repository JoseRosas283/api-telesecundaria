using Telesecundaria.DTOs.Entregas;

namespace Telesecundaria.Services.Interfaces
{
    public interface IEntregasService
    {
        Task<EntregaResponseDTO> InicializarEntregaAsync(EntregaRequestDTO dto);
    }
}
