using Telesecundaria.DTOs.AdjuncionesOriginales;

namespace Telesecundaria.Services.Interfaces
{
    public interface IAdjuncionesOriginalesService
    {
        Task<AdjuncionOriginalResponseDTO> RegistrarAdjuncionOriginalAsync(AdjuncionOriginalRequestDTO dto);
        Task<DetalleAdjuncionOriginalResponseDTO> RegistrarDetalleAsync(DetalleAdjuncionOriginalRequestDTO dto);
        Task<ProgresoAdjuncionOriginalDTO> ObtenerProgresoAsync(string claveAdjOriginal);
    }
}
