using Telesecundaria.DTOs;
using Telesecundaria.DTOs.GaleriaImagenes;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IGaleriaImagenesService
    {
        Task<IEnumerable<GaleriaImagenResponseDTO>> ListarImagenesAsync();
        Task<GaleriaImagenResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<GaleriaImagenResponseDTO> RegistrarImagenAsync(GaleriaImagenRequestDTO dto);
        Task ActualizarImagenAsync(string clave, GaleriaImagenUpdateDTO dto);
        Task EliminarImagenAsync(string clave);
    }
}
