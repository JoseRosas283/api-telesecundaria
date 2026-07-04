using Telesecundaria.DTOs.TutorAspirante;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface ITutorAspiranteRepository
    {
        Task<IEnumerable<TutorAspiranteEntity>> ListarTutoresAsync();
        Task<TutorAspiranteEntity?> ObtenerPorIdAsync(string clave);
        Task<TutorAspiranteEntity?> ObtenerPorCurpAsync(string curp);
        Task<TutorAspiranteEntity> RegistrarTutorAsync(TutorAspiranteRequestDTO dto);
        Task ActualizarTutorAsync(string clave, TutorAspiranteUpdateDTO dto);
        Task EliminarTutorAsync(string clave);
    }
}
