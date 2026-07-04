using Telesecundaria.DTOs.Pagos;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IPagosRepository
    {
        Task<IEnumerable<PagosEntity>> ListarPagosAsync();
        Task<PagosEntity?> ObtenerPorIdAsync(string clave);
        Task RegistrarPagoAsync(PagoRequestDTO dto);
    }
}
