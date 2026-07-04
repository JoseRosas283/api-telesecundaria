using Telesecundaria.DTOs.Publicaciones;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IPublicacionesRepository
    {
        Task<IEnumerable<PublicacionesEntity>> ListarPublicacionesAsync();
        Task<PublicacionesEntity?> ObtenerPorIdAsync(string clave);
        Task<PublicacionesEntity> RegistrarPublicacionAsync(PublicacionRequestDTO dto);
        Task ActualizarPublicacionAsync(string clave, PublicacionesEntity publicacion);
        Task EliminarPublicacionAsync(string clave);
    }
}
