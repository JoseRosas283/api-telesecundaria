using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IEnviosRepository
    {
        Task<IEnumerable<EnviosEntity>> ListarTodosAsync();
        Task<IEnumerable<EnviosEntity>> ListarPendientesAsync();
        Task<EnviosEntity?> ObtenerPorIdAsync(string clave);
        Task MarcarComoEnviadoAsync(string claveEnvio);
        Task MarcarComoFallidoAsync(string claveEnvio, string errorLog);
    }
}
