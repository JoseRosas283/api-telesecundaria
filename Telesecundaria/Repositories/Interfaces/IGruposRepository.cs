using Telesecundaria.DTOs.Grupos;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IGruposRepository
    {
        Task<IEnumerable<GruposEntity>> ListarGruposAsync();
        Task<GruposEntity?> ObtenerPorIdAsync(string clave);
        Task<GruposEntity> RegistrarGrupoAsync(GrupoRequestDTO dto);
    }
}
