using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Requisitos;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IRequisitosService
    {
        Task<IEnumerable<RequisitoResponseDTO>> ListarRequisitosAsync();
        Task<RequisitoResponseDTO?> ObtenerPorIdAsync(string claveRequisito);
        Task<RequisitoResponseDTO> ConfigurarRequisitoEtapaAsync(RequisitoRequestDTO dto);
    }
}
