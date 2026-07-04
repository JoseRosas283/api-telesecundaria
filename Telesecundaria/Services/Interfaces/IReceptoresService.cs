using Telesecundaria.DTOs.Receptores;

namespace Telesecundaria.Services.Interfaces
{
    public interface IReceptoresService
    {
        Task<IEnumerable<ReceptorResponseDTO>> ListarAsync();
        Task<ReceptorResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<ReceptorResponseDTO> RegistrarAsync(ReceptorRequestDTO dto);
        Task<ReceptorResponseDTO> ActualizarAsync(string claveReceptor, ReceptorUpdateDTO dto);
        Task EliminarAsync(string claveReceptor);
    }
}
