using Telesecundaria.DTOs.Inscripciones;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IInscripcionesRepository
    {
        Task<IEnumerable<InscripcionesEntity>> ListarInscripcionesAsync();
        Task<InscripcionesEntity?> ObtenerPorIdAsync(string clave);
        Task RealizarInscripcionAsync(InscripcionRequestDTO dto);
    }
}
