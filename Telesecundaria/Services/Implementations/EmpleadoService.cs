using Telesecundaria.DTOs.Empleados.Request;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadosRepository _repository;

        public EmpleadoService(IEmpleadosRepository repository)
        {
            _repository = repository;

        }

        public async Task<IEnumerable<EmpleadosEntity>> GetAllAsync()
        {
            var empleados = await _repository.GetAllAsync();
            return empleados;

        }

        public async Task<EmpleadosEntity?> GetByIdAsync(string claveEmpleado)
        {
            if (string.IsNullOrWhiteSpace(claveEmpleado))
                throw new ArgumentException("La clave del Empleado no puede estar vacia");

            var empleado = await _repository.GetByIdAsync(claveEmpleado);

            return empleado;
        }


        public async Task<EmpleadosEntity> CreateAsync(EmpleadoCreateRequest request)
        {
            // Validaciones de strings
            if (string.IsNullOrWhiteSpace(request.ClaveExpediente))
                throw new ArgumentException("La clave del expediente es obligatoria");

            if (string.IsNullOrWhiteSpace(request.TipoContrato))
                throw new ArgumentException("El tipo de contrato es obligatorio: Planta o Temporal");

            if (string.IsNullOrWhiteSpace(request.Telefono))
                throw new ArgumentException("El número telefónico es obligatorio");


            if (request.FechaContratacion == default(DateTime))
                throw new ArgumentException("La fecha de contratación es obligatoria");


            if (request.FechaContratacion > DateTime.Now)
                throw new ArgumentException("La fecha de contratación no puede ser futura");

            if (request.TipoContrato != "Planta" && request.TipoContrato != "Temporal")
                throw new ArgumentException("El tipo de contrato solo puede ser Planta o Temporal");

            return await _repository.CreateAsync(request);
        }

        public async Task UpdateAsync(string claveEmpleado, EmpleadoUpdateRequest request)
        {

            var existente = await _repository.GetByIdAsync(claveEmpleado);

            if (existente == null)
                throw new ArgumentException($"La clave no existe: {claveEmpleado}");

            existente.TipoContrato = request.TipoContrato;

            existente.Telefono = request.Telefono;

            existente.EstatusLaboral = request.EstatusLaboral;

            await _repository.UpdateAsync(claveEmpleado, request);

        }

        public async Task DeleteAsync(string claveEmpleado)
        {
            var usuario = await _repository.GetByIdAsync(claveEmpleado);
            if (usuario == null)
                throw new KeyNotFoundException("Intento de eliminar un Empleado inexistente.");


            await _repository.DeleteAsync(claveEmpleado);
        }

    }
}
