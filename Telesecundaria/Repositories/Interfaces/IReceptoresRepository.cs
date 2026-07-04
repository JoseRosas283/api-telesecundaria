using Telesecundaria.DTOs.Receptores;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IReceptoresRepository
    {
        Task<IEnumerable<ReceptoresEntity>> ListarAsync();
        Task<ReceptoresEntity?> ObtenerPorIdAsync(string clave);
        Task RegistrarAsync(ReceptorRequestDTO dto);
        Task ActualizarAsync(string claveReceptor, ReceptorUpdateDTO dto);
        Task EliminarAsync(string claveReceptor);
    }
}
