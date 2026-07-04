using Telesecundaria.DTOs.CitasInscripcion;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface ICitasInscripcionRepository
    {
        Task<IEnumerable<CitasInscripcionEntity>> ListarCitasAsync();
        Task<CitasInscripcionEntity?> ObtenerPorIdAsync(string claveCita);
        Task AgendarCitaAsync(CitaInscripcionRequestDTO dto);
        Task<CitasInscripcionEntity?> ObtenerPorRevisionAsync(string claveRevision);
    }
}
