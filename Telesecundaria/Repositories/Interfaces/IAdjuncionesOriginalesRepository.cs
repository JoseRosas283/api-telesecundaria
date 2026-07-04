using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IAdjuncionesOriginalesRepository
    {
        Task<string> CrearAdjuncionOriginalAsync(string claveEntrega, string claveUsuario);
        Task<AdjuncionesOriginalesEntity?> ObtenerAdjuncionOriginalAsync(string claveAdjOriginal);
        Task<AdjuncionesOriginalesEntity?> ObtenerPorEntregaAsync(string claveEntrega);
        Task InsertarDetalleAdjuncionOriginalAsync(string claveAdjOriginal, string claveDocAspirante);
        Task<List<DetalleAdjuncionOriginalEntity>> ObtenerDetallesAsync(string claveAdjOriginal);
        Task<int> ContarRequisitosInscripcionAsync();
        Task<string?> ObtenerEstadoEntregaAsync(string claveEntrega);
    }
}
