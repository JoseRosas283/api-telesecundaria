using Telesecundaria.DTOs.Expedientes.Request;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class ExpedienteService : IExpedienteService
    {
        private readonly IExpedientesRepository _repository;

        public ExpedienteService(IExpedientesRepository repository)
        {

            _repository = repository;

        }


        public async Task<IEnumerable<ExpedientesEntity>> GetAllAsync()
        {
            var expediente = await _repository.GetAllAsync();
            return expediente;

        }

        public async Task<ExpedientesEntity?> GetByIdAsync(string claveExpediente)
        {
            if (string.IsNullOrWhiteSpace(claveExpediente))
                throw new ArgumentException("La clave del expediente no puede estar vacia");

            var expediente = await _repository.GetByIdAsync(claveExpediente);

            return expediente;


        }


        public async Task<ExpedientesEntity> CreateAsync(ExpedienteCreateRequest request)
        {

            return await _repository.CreateAsync(request);

        }


        public async Task UpdateAsync(string claveExpediente, ExpedienteUpdateRequest request)
        {
            var existente = await _repository.GetByIdAsync(claveExpediente);

            if (existente == null)
                throw new ArgumentException($"La clave no existe: {claveExpediente}");

            await _repository.UpdateAsync(claveExpediente, request);

        }

        public async Task DeleteAsync(string claveExpediente)
        {
            if (string.IsNullOrWhiteSpace(claveExpediente))
                throw new ArgumentException("la clave del expediente no puede estar vacia.");

            await _repository.DeleteAsync(claveExpediente);
        }
    }
}
