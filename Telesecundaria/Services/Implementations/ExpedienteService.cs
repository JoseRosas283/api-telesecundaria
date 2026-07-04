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

            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("El nombre del expediente es obligatorio");

            if (string.IsNullOrWhiteSpace(request.ApellidoPaterno))
                throw new ArgumentException("El apellido paterno es obligatorio");

            if (string.IsNullOrWhiteSpace(request.ApellidoMaterno))
                throw new ArgumentException("El apellido materno es obligatorio");

            if (string.IsNullOrWhiteSpace(request.Curp))
                throw new ArgumentException("La curp debe ser obligatoria");

            if (string.IsNullOrWhiteSpace(request.TipoTitular))
                throw new ArgumentException("El tipo titular debe ser obligatoria");

            return await _repository.CreateAsync(request);

        }


        public async Task UpdateAsync(string claveExpediente, ExpedienteUpdateRequest request)
        {

            if (string.IsNullOrWhiteSpace(claveExpediente))
                throw new ArgumentException("La clave del expediente no puede estar vacía.");

            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.ApellidoPaterno))
                throw new ArgumentException("El apellido paterno es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.ApellidoMaterno))
                throw new ArgumentException("El apellido materno es obligatorio");

            if (string.IsNullOrWhiteSpace(request.Curp))
                throw new ArgumentException("La CURP es obligatoria.");

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
