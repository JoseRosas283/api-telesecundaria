using Telesecundaria.DTOs.CiclosEscolares;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface ICiclosEscolaresRepository
    {
        Task<IEnumerable<CiclosEscolaresEntity>> ListarCiclosAsync();
        Task<CiclosEscolaresEntity?> ObtenerPorIdAsync(string clave);
        Task AbrirCicloAsync(CicloEscolarRequestDTO dto);
    }
}
