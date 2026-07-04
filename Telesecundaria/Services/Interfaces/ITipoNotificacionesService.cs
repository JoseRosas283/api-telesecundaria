using Telesecundaria.DTOs.TipoNotificaciones;

namespace Telesecundaria.Services.Interfaces
{
    public interface ITipoNotificacionesService
    {
        Task<IEnumerable<TipoNotificacionResponseDTO>> ListarAsync();
        Task<TipoNotificacionResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<TipoNotificacionResponseDTO> RegistrarAsync(TipoNotificacionRequestDTO dto);

    }
}
