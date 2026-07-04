using Telesecundaria.DTOs.DestinoNotificacion;

namespace Telesecundaria.Services.Interfaces
{
    public interface IDestinoNotificacionService
    {
        Task<IEnumerable<DestinoNotificacionResponseDTO>> ListarAsync();
        Task<DestinoNotificacionResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<DestinoNotificacionResponseDTO> RegistrarAsync(DestinoNotificacionRequestDTO dto);
    }
}
