using Telesecundaria.DTOs.Requisitos;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IRequisitosRepository
    {
        Task<IEnumerable<RequisitosEntity>> ListarRequisitosAsync();
        Task<RequisitosEntity?> ObtenerPorIdAsync(string claveRequisito);
        Task ConfigurarRequisitoEtapaAsync(RequisitoRequestDTO dto);
    }
}
