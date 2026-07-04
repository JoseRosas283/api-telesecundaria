using Telesecundaria.DTOs.Expedientes.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IExpedientesRepository
    {
        Task<IEnumerable<ExpedientesEntity>> GetAllAsync();

        Task<ExpedientesEntity> GetByIdAsync(string claveExpediente);


        Task<ExpedientesEntity> CreateAsync(ExpedienteCreateRequest request);

        Task UpdateAsync(string claveExpediente, ExpedienteUpdateRequest request);

        Task DeleteAsync(string claveExpediente);
    }
}
