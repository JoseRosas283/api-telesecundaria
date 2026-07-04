using Telesecundaria.DTOs.CitasInscripcion;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class CitasInscripcionService : ICitasInscripcionService
    {
        private readonly ICitasInscripcionRepository _repository;

        public CitasInscripcionService(ICitasInscripcionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CitaInscripcionResponseDTO>> ListarCitasAsync()
        {
            var citas = await _repository.ListarCitasAsync();
            return citas.Select(MapearResponse);
        }

        public async Task<CitaInscripcionResponseDTO?> ObtenerPorIdAsync(string claveCita)
        {
            if (string.IsNullOrWhiteSpace(claveCita))
                throw new ArgumentException("La clave de la cita es obligatoria.");

            var cita = await _repository.ObtenerPorIdAsync(claveCita);
            if (cita == null)
                return null;

            return MapearResponse(cita);
        }

        public async Task<CitaInscripcionResponseDTO> AgendarCitaAsync(CitaInscripcionRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveRevision))
                throw new ArgumentException("La clave de la revisión es obligatoria.");

            if (dto.FechaCita == default)
                throw new ArgumentException("La fecha de la cita es obligatoria.");

            if (dto.HoraCita == default)
                throw new ArgumentException("La hora de la cita es obligatoria.");

            await _repository.AgendarCitaAsync(dto);

            var citaCreada = await _repository.ObtenerPorRevisionAsync(dto.ClaveRevision);
            if (citaCreada == null)
                throw new Exception("La cita fue agendada pero no se pudo recuperar.");

            return MapearResponse(citaCreada);
        }

        private static CitaInscripcionResponseDTO MapearResponse(CitasInscripcionEntity c) =>
            new CitaInscripcionResponseDTO
            {
                ClaveCita = c.ClaveCita,
                ClaveRevision = c.ClaveRevision,
                ClaveTutorAspirante = c.ClaveTutorAspirante,
                FechaCita = c.FechaCita,
                HoraCita = c.HoraCita,
                EstadoCita = c.EstadoCita,
                Observaciones = c.Observaciones,
                CreateAt = c.CreateAt
            };
    }
}
