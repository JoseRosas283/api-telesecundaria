using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IEntregasRepository
    {
        Task<string> CrearEntregaAsync(string claveCita, string claveUsuario);
        Task<EntregasEntity?> ObtenerEntregaAsync(string claveEntrega);
        Task<EntregasEntity?> ObtenerPorCitaAsync(string claveCita);
    }
}
