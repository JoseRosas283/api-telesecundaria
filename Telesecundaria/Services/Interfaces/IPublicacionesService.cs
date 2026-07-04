using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Publicaciones;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IPublicacionesService
    {
        Task<IEnumerable<PublicacionResponseDTO>> ListarPublicacionesAsync();
        Task<PublicacionResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<PublicacionResponseDTO> RegistrarPublicacionAsync(PublicacionRequestDTO dto);
        Task ActualizarPublicacionAsync(string clave, PublicacionUpdateDTO dto);
        Task EliminarPublicacionAsync(string clave);
    }
}
