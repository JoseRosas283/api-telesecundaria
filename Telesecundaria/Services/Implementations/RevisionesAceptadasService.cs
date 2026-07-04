using Telesecundaria.DTOs.RevisionesAceptadas;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class RevisionesAceptadasService : IRevisionesAceptadasService
    {
        private readonly IRevisionesAceptadasRepository _repository;

        public RevisionesAceptadasService(IRevisionesAceptadasRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RevisionAceptadaResponseDTO>> ListarPendientesAsync()
        {
            var pendientes = await _repository.ListarPendientesAsync();
            return pendientes.Select(r => MapearResponse(r));
        }

        public async Task<RevisionAceptadaResponseDTO?> ObtenerPorIdAsync(string claveRevision)
        {
            if (string.IsNullOrWhiteSpace(claveRevision))
                throw new ArgumentException("La clave de la revisión es obligatoria.");

            var aceptada = await _repository.ObtenerPorIdAsync(claveRevision);
            if (aceptada == null)
                return null;

            return MapearResponse(aceptada);
        }

        public async Task RegistrarAceptacionAsync(RevisionAceptadaRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveRevision))
                throw new ArgumentException("La clave de la revisión es obligatoria.");

            await _repository.RegistrarAceptacionAsync(dto);
        }

        private static RevisionAceptadaResponseDTO MapearResponse(RevisionesAceptadasEntity r) =>
            new RevisionAceptadaResponseDTO
            {
                ClaveRevisionAceptada = r.ClaveRevisionAceptada,
                ClaveRevision = r.ClaveRevision,
                ClaveReceptor = r.ClaveReceptor,
                ClaveConvocatoria = r.ClaveConvocatoria,
                FechaAceptacion = r.FechaAceptacion,
                Estado = r.Estado
            };
    }
}
