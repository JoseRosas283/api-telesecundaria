using Telesecundaria.DTOs.TipoDocumentos;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface ITipoDocumentosRepository
    {
        Task<IEnumerable<TipoDocumentosEntity>> ListarTipoDocumentosAsync();
        Task<TipoDocumentosEntity?> ObtenerPorIdAsync(string clave);
        Task InsertarTipoDocumentoAsync(TipoDocumentoRequestDTO dto);
    }
}
