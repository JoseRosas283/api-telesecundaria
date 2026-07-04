using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Grupos;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IGruposService
    {
        Task<IEnumerable<GrupoResponseDTO>> ListarGruposAsync();
        Task<GrupoResponseDTO?> ObtenerPorIdAsync(string clave);
        Task<GrupoResponseDTO> RegistrarGrupoAsync(GrupoRequestDTO dto);
    }
}
