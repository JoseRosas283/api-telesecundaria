using Telesecundaria.DTOs.Tutores;

namespace Telesecundaria.Services.Interfaces
{
    public interface ITutoresService
    {
        Task<IEnumerable<TutorResponseDTO>> ListarTutoresAsync();
        Task<TutorResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<TutorResponseDTO> RegistrarTutorAsync(TutorRequestDTO dto);
        Task<TutorResponseDTO> ActualizarTutorAsync(string claveTutor, TutorUpdateDTO dto);
        Task EliminarTutorAsync(string claveTutor);
    }
}
