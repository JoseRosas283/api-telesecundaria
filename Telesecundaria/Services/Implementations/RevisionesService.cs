using Telesecundaria.DTOs.Revisiones;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class RevisionesService : IRevisionesService
    {
        private readonly IRevisionesRepository _repository;

        public RevisionesService(IRevisionesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RevisionResponseDTO>> ListarRevisionesAsync()
        {
            var revisiones = await _repository.ListarRevisionesAsync();
            return revisiones.Select(r => MapearResponse(r));
        }

        public async Task<RevisionResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave de la revisión es obligatoria.");

            var revision = await _repository.ObtenerPorIdAsync(clave);
            if (revision == null)
                return null;

            return MapearResponse(revision);
        }

        public async Task<RevisionResponseDTO> ProcesarRevisionAsync(RevisionRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveUsuario))
                throw new ArgumentException("La clave del usuario es obligatoria.");

            await _repository.ProcesarRevisionAsync(dto);

            var revisionCreada = await _repository.ObtenerUltimaPorUsuarioAsync(dto.ClaveUsuario);
            return MapearResponse(revisionCreada!);
        }

        public async Task<RevisionResponseDTO> CerrarRevisionAsync(string claveRevision)
        {
            if (string.IsNullOrWhiteSpace(claveRevision))
                throw new ArgumentException("La clave de la revisión es obligatoria.");

            var existe = await _repository.ObtenerPorIdAsync(claveRevision);
            if (existe == null)
                throw new InvalidOperationException($"La revisión con clave {claveRevision} no existe.");

            await _repository.CerrarRevisionAsync(claveRevision);

            var revisionCerrada = await _repository.ObtenerPorIdAsync(claveRevision);
            return MapearResponse(revisionCerrada!);
        }

        private static RevisionResponseDTO MapearResponse(RevisionesEntity r) =>
            new RevisionResponseDTO
            {
                ClaveRevision = r.ClaveRevision,
                ClaveAdjuncion = r.ClaveAdjuncion,
                ClaveUsuario = r.ClaveUsuario,
                EstatusRevision = r.EstatusRevision,
                EstadoOperativo = r.EstadoOperativo,
                ObservacionGeneral = r.ObservacionGeneral,
                FechaRevision = r.FechaRevision
            };
    }
}
