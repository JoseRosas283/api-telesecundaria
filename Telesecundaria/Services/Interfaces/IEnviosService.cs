using Telesecundaria.DTOs.Envios;

namespace Telesecundaria.Services.Interfaces
{
    public interface IEnviosService
    {
        Task<IEnumerable<EnvioResponseDTO>> ListarTodosAsync();
        Task<EnvioResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<ProcesarEnviosResponseDTO> ProcesarPendientesAsync();
    }
}
