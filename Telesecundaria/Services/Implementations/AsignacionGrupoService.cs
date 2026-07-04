using Telesecundaria.DTOs;
using Telesecundaria.DTOs.AsignacionGrupo;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class AsignacionGrupoService : IAsignacionGrupoService
    {
        private readonly IAsignacionGrupoRepository _repository;

        public AsignacionGrupoService(IAsignacionGrupoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AsignacionGrupoResponseDTO>> ListarAsignacionesAsync()
        {
            var asignaciones = await _repository.ListarAsignacionesAsync();
            return asignaciones.Select(a => MapearResponse(a));
        }

        public async Task<AsignacionGrupoResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave de la asignación es obligatoria.");

            var asignacion = await _repository.ObtenerPorIdAsync(clave);
            if (asignacion == null)
                return null;

            return MapearResponse(asignacion);
        }

        public async Task AsignarAlumnoGrupoAsync(AsignacionGrupoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveAlumno))
                throw new ArgumentException("La clave del alumno es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.ClaveGrupo))
                throw new ArgumentException("La clave del grupo es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.ClaveUsuario))
                throw new ArgumentException("La clave del usuario es obligatoria.");

            await _repository.AsignarAlumnoGrupoAsync(dto);
        }

        private static AsignacionGrupoResponseDTO MapearResponse(AsignacionGrupoEntity a) =>
            new AsignacionGrupoResponseDTO
            {
                ClaveAsignacion = a.ClaveAsignacion,
                ClaveAlumno = a.ClaveAlumno,
                ClaveGrupo = a.ClaveGrupo,
                ClaveUsuario = a.ClaveUsuario,
                ClaveCiclo = a.ClaveCiclo,
                FechaAsignacion = a.FechaAsignacion,
                Estatus = a.Estatus
            };
    }
}
