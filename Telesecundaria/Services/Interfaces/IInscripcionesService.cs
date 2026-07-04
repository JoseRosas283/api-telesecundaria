using Telesecundaria.DTOs.Inscripciones;

namespace Telesecundaria.Services.Interfaces
{
    public interface IInscripcionesService
    {
        Task<IEnumerable<InscripcionResponseDTO>> ListarInscripcionesAsync();
        Task<InscripcionResponseDTO?> ObtenerPorIdAsync(string clave);
        Task RealizarInscripcionAsync(InscripcionRequestDTO dto);
    }
}
