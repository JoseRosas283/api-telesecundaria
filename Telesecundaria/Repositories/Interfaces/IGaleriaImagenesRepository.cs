using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IGaleriaImagenesRepository
    {
        Task<IEnumerable<GaleriaImagenesEntity>> ListarImagenesAsync();
        Task<GaleriaImagenesEntity?> ObtenerPorIdAsync(string clave);
        Task<GaleriaImagenesEntity> RegistrarImagenAsync(GaleriaImagenesEntity imagen);
        Task ActualizarImagenAsync(string clave, GaleriaImagenesEntity imagen);
        Task EliminarImagenAsync(string clave);
    }
}
