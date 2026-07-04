using Telesecundaria.DTOs.Convocatorias;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IConvocatoriasRepository
    {
        Task<IEnumerable<ConvocatoriasEntity>> ListarConvocatoriasAsync();
        Task<ConvocatoriasEntity?> ObtenerPorIdAsync(string clave);
        Task<ConvocatoriasEntity> RegistrarConvocatoriaAsync(ConvocatoriaRequestDTO dto);
        Task ActualizarConvocatoriaAsync(string clave, ConvocatoriaUpdateDTO dto);
        Task EliminarConvocatoriaAsync(string clave, string nombreUsuario);
    }
}
