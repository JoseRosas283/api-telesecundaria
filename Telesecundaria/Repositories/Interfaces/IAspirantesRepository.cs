using Telesecundaria.DTOs.Aspirantes;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IAspirantesRepository
    {
        Task<IEnumerable<AspirantesEntity>> ListarAspirantesAsync();
        Task<AspirantesEntity?> ObtenerPorIdAsync(string clave);
        Task<AspirantesEntity?> ObtenerPorCurpAsync(string curp);
        Task<IEnumerable<AspirantesEntity>> ObtenerPorConvocatoriaAsync(string claveConvocatoria);
        Task<AspirantesEntity> RegistrarAspiranteAsync(AspiranteRequestDTO aspirante);
        Task ActualizarAspiranteAsync(string clave, AspiranteUpdateDTO dto);
        Task EliminarAspiranteAsync(string clave);
    }
}
