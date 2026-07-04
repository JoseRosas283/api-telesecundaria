using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Convocatorias;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IConvocatoriasService
    {
        Task<IEnumerable<ConvocatoriaResponseDTO>> ListarConvocatoriasAsync();
        Task<ConvocatoriaResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<ConvocatoriaResponseDTO> RegistrarConvocatoriaAsync(ConvocatoriaRequestDTO dto);
        Task ActualizarConvocatoriaAsync(string clave, ConvocatoriaUpdateDTO dto);
        Task EliminarConvocatoriaAsync(string clave, string nombreUsuario);
    }
}
