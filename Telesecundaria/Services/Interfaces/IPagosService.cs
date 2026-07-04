using Telesecundaria.DTOs.Pagos;

namespace Telesecundaria.Services.Interfaces
{
    public interface IPagosService
    {
        Task<IEnumerable<PagoResponseDTO>> ListarPagosAsync();
        Task<PagoResponseDTO?> ObtenerPorIdAsync(string clave);
        Task RegistrarPagoAsync(PagoRequestDTO dto);
    }
}
