using Telesecundaria.DTOs.Tutores;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface ITutoresRepository
    {
        Task<IEnumerable<TutoresEntity>> ListarTutoresAsync();
        Task<TutoresEntity?> ObtenerPorIdAsync(string clave);
        Task<TutoresEntity?> ObtenerPorCurpAsync(string curp);
        Task RegistrarTutorAsync(TutorRequestDTO dto);
        Task ActualizarTutorAsync(string claveTutor, TutorUpdateDTO dto);
        Task EliminarTutorAsync(string claveTutor);
    }
}
