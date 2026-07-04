using Telesecundaria.DTOs;
using Telesecundaria.DTOs.TipoDocumentos;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface ITipoDocumentosService
    {
        Task<IEnumerable<TipoDocumentoResponseDTO>> ListarTipoDocumentosAsync();
        Task<TipoDocumentoResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<TipoDocumentoResponseDTO> InsertarTipoDocumentoAsync(TipoDocumentoRequestDTO dto);
    }
}
