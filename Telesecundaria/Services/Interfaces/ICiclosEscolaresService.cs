using Telesecundaria.DTOs.CiclosEscolares;

namespace Telesecundaria.Services.Interfaces
{
    public interface ICiclosEscolaresService
    {
        Task<IEnumerable<CicloEscolarResponseDTO>> ListarCiclosAsync();
        Task<CicloEscolarResponseDTO?> ObtenerPorIdAsync(string clave);
        Task AbrirCicloAsync(CicloEscolarRequestDTO dto);
    }
}
