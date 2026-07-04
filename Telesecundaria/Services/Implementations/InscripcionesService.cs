using Telesecundaria.DTOs.Inscripciones;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class InscripcionesService : IInscripcionesService
    {
        private readonly IInscripcionesRepository _repository;

        public InscripcionesService(IInscripcionesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<InscripcionResponseDTO>> ListarInscripcionesAsync()
        {
            var inscripciones = await _repository.ListarInscripcionesAsync();
            return inscripciones.Select(i => MapearResponse(i));
        }

        public async Task<InscripcionResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave de la inscripción es obligatoria.");

            var inscripcion = await _repository.ObtenerPorIdAsync(clave);
            if (inscripcion == null)
                return null;

            return MapearResponse(inscripcion);
        }

        public async Task RealizarInscripcionAsync(InscripcionRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveAlumno))
                throw new ArgumentException("La clave del alumno es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            await _repository.RealizarInscripcionAsync(dto);
        }

        private static InscripcionResponseDTO MapearResponse(InscripcionesEntity i) =>
            new InscripcionResponseDTO
            {
                ClaveInscripcion = i.ClaveInscripcion,
                ClaveAlumno = i.ClaveAlumno,
                ClaveCiclo = i.ClaveCiclo,
                ClavePeriodo = i.ClavePeriodo,
                ClaveGrupo = i.ClaveGrupo,
                ClavePago = i.ClavePago,
                FechaInscripcion = i.FechaInscripcion,
                EstatusInscripcion = i.EstatusInscripcion,
                Observaciones = i.Observaciones
            };
    }
}
