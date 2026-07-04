using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Aspirantes;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IAspirantesService
    {
        Task<IEnumerable<AspiranteResponseDTO>> ListarAspirantesAsync();
        Task<AspiranteResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<AspiranteResponseDTO?> ObtenerPorCurpAsync(string curp);
        Task<IEnumerable<AspiranteResponseDTO>> ObtenerPorConvocatoriaAsync(string claveConvocatoria);
        Task<AspiranteResponseDTO> RegistrarAspiranteAsync(AspiranteRequestDTO dto);
        Task ActualizarAspiranteAsync(string clave, AspiranteUpdateDTO dto);
        Task EliminarAspiranteAsync(string clave);
    }
}
