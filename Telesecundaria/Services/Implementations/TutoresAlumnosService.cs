using Telesecundaria.DTOs.TutoresAlumnos;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class TutoresAlumnosService : ITutoresAlumnosService
    {
        private readonly ITutoresAlumnosRepository _repository;

        public TutoresAlumnosService(ITutoresAlumnosRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TutorAlumnoResponseDTO>> ListarAsync()
        {
            var relaciones = await _repository.ListarAsync();
            return relaciones.Select(r => MapearResponse(r));
        }

        public async Task<TutorAlumnoResponseDTO?> ObtenerPorIdAsync(string claveAlumno, string claveTutor)
        {
            if (string.IsNullOrWhiteSpace(claveAlumno) || string.IsNullOrWhiteSpace(claveTutor))
                throw new ArgumentException("Las claves de alumno y tutor son obligatorias.");

            var relacion = await _repository.ObtenerPorIdAsync(claveAlumno, claveTutor);
            if (relacion == null)
                return null;

            return MapearResponse(relacion);
        }

        public async Task<TutorAlumnoResponseDTO> AsignarTutorAlumnoAsync(TutorAlumnoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveAlumno))
                throw new ArgumentException("La clave del alumno es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.ClaveTutor))
                throw new ArgumentException("La clave del tutor es obligatoria.");

            await _repository.AsignarTutorAlumnoAsync(dto.ClaveAlumno, dto.ClaveTutor);

            var creado = await _repository.ObtenerPorIdAsync(dto.ClaveAlumno, dto.ClaveTutor);
            return MapearResponse(creado!);
        }

        private static TutorAlumnoResponseDTO MapearResponse(TutoresAlumnosEntity ta) =>
            new TutorAlumnoResponseDTO
            {
                ClaveAlumno = ta.ClaveAlumno,
                ClaveTutor = ta.ClaveTutor,
                FechaInicio = ta.FechaInicio,
                FechaBaja = ta.FechaBaja
            };
    }
}
