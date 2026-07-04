using Telesecundaria.DTOs;
using Telesecundaria.DTOs.AsignacionGrupo;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IAsignacionGrupoService
    {
        Task<IEnumerable<AsignacionGrupoResponseDTO>> ListarAsignacionesAsync();
        Task<AsignacionGrupoResponseDTO?> ObtenerPorIdAsync(string clave);
        Task AsignarAlumnoGrupoAsync(AsignacionGrupoRequestDTO dto);
    }
}
