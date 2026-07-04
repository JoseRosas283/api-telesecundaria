using System.Globalization;
using Telesecundaria.DTOs.CiclosEscolares;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class CiclosEscolaresService : ICiclosEscolaresService
    {
        private readonly ICiclosEscolaresRepository _repository;

        public CiclosEscolaresService(ICiclosEscolaresRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CicloEscolarResponseDTO>> ListarCiclosAsync()
        {
            var ciclos = await _repository.ListarCiclosAsync();
            return ciclos.Select(c => MapearResponse(c));
        }

        public async Task<CicloEscolarResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del ciclo escolar es obligatoria.");

            var ciclo = await _repository.ObtenerPorIdAsync(clave);
            if (ciclo == null)
                return null;

            return MapearResponse(ciclo);
        }

        public async Task AbrirCicloAsync(CicloEscolarRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FechaInicio) || string.IsNullOrWhiteSpace(dto.FechaFin))
                throw new ArgumentException("Las fechas de inicio y fin son obligatorias.");

            if (!DateTime.TryParseExact(dto.FechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaInicio))
                throw new ArgumentException("La fecha de inicio no tiene un formato válido. Use DD/MM/YYYY.");

            if (!DateTime.TryParseExact(dto.FechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fechaFin))
                throw new ArgumentException("La fecha de fin no tiene un formato válido. Use DD/MM/YYYY.");

            if (fechaFin <= fechaInicio)
                throw new ArgumentException("La fecha de fin debe ser mayor a la fecha de inicio.");

            await _repository.AbrirCicloAsync(dto);
        }

        private static CicloEscolarResponseDTO MapearResponse(CiclosEscolaresEntity c) =>
            new CicloEscolarResponseDTO
            {
                ClaveCiclo = c.ClaveCiclo,
                NombreCiclo = c.NombreCiclo,
                FechaInicio = c.FechaInicio,
                FechaFin = c.FechaFin,
                Estatus = c.Estatus
            };
    }
}
