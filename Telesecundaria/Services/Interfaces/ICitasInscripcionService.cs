using Telesecundaria.DTOs.CitasInscripcion;

namespace Telesecundaria.Services.Interfaces
{
    public interface ICitasInscripcionService
    {
        Task<IEnumerable<CitaInscripcionResponseDTO>> ListarCitasAsync();
        Task<CitaInscripcionResponseDTO?> ObtenerPorIdAsync(string claveCita);
        Task<CitaInscripcionResponseDTO> AgendarCitaAsync(CitaInscripcionRequestDTO dto);
    }
}
