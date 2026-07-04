using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Adjunciones;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IAdjuncionesService
    {
        Task<AdjuncionResponseDTO> RegistrarAdjuncionAsync(AdjuncionRequestDTO dto);
        Task<DocumentoAdjuntadoDTO> RegistrarDocumentoTempAsync(DocumentoTempRequestDTO dto);
        Task<EstadoDocumentosResponseDTO> ObtenerEstadoDocumentosAsync(string claveAspirante);
        Task<AdjuncionResponseDTO> FinalizarAdjuncionAsync(FinalizarAdjuncionRequestDTO dto);
    }
}
