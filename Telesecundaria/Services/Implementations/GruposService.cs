using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Grupos;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class GruposService : IGruposService
    {
        private readonly IGruposRepository _repository;

        public GruposService(IGruposRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GrupoResponseDTO>> ListarGruposAsync()
        {
            var grupos = await _repository.ListarGruposAsync();
            return grupos.Select(g => MapearResponse(g));
        }

        public async Task<GrupoResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del grupo es obligatoria.");

            var grupo = await _repository.ObtenerPorIdAsync(clave);
            if (grupo == null)
                return null;

            return MapearResponse(grupo);
        }

        public async Task<GrupoResponseDTO> RegistrarGrupoAsync(GrupoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Grado) || !new[] { "1", "2", "3" }.Contains(dto.Grado))
                throw new ArgumentException("El grado debe ser '1', '2' o '3'.");

            if (string.IsNullOrWhiteSpace(dto.Seccion) || !new[] { "A", "B" }.Contains(dto.Seccion))
                throw new ArgumentException("La sección debe ser 'A' o 'B'.");

            if (dto.CapacidadMaxima <= 0)
                throw new ArgumentException("La capacidad máxima debe ser mayor a cero.");

            if (string.IsNullOrWhiteSpace(dto.Generacion))
                throw new ArgumentException("La generación es obligatoria.");

            var grupo = await _repository.RegistrarGrupoAsync(dto);
            return MapearResponse(grupo);
        }

        private static GrupoResponseDTO MapearResponse(GruposEntity g) =>
            new GrupoResponseDTO
            {
                ClaveGrupo = g.ClaveGrupo,
                Grado = g.Grado,
                Seccion = g.Seccion,
                CapacidadMaxima = g.CapacidadMaxima,
                Generacion = g.Generacion,
                Estado = g.Estado
            };
    }
}
