using Telesecundaria.DTOs.AsignacionGrupo;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IAsignacionGrupoRepository
    {
        Task<IEnumerable<AsignacionGrupoEntity>> ListarAsignacionesAsync();
        Task<AsignacionGrupoEntity?> ObtenerPorIdAsync(string clave);
        Task AsignarAlumnoGrupoAsync(AsignacionGrupoRequestDTO dto);
    }
}
