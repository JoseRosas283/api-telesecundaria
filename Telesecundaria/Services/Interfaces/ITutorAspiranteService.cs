using Telesecundaria.DTOs;
using Telesecundaria.DTOs.TutorAspirante;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface ITutorAspiranteService
    {
        Task<IEnumerable<TutorAspiranteResponseDTO>> ListarTutoresAsync();
        Task<TutorAspiranteResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<TutorAspiranteResponseDTO?> ObtenerPorCurpAsync(string curp);
        Task<TutorAspiranteResponseDTO> RegistrarTutorAsync(TutorAspiranteRequestDTO dto);
        Task ActualizarTutorAsync(string clave, TutorAspiranteUpdateDTO dto);
        Task EliminarTutorAsync(string clave);
    }
}
