using Telesecundaria.DTOs.DetalleRevision;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class DetalleRevisionService : IDetalleRevisionService
    {
        private readonly IDetalleRevisionRepository _repository;

        public DetalleRevisionService(IDetalleRevisionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DetalleRevisionResponseDTO>> ListarPorRevisionAsync(string claveRevision)
        {
            if (string.IsNullOrWhiteSpace(claveRevision))
                throw new ArgumentException("La clave de la revisión es obligatoria.");

            var detalles = await _repository.ListarPorRevisionAsync(claveRevision);
            return detalles.Select(d => MapearResponse(d));
        }

        public async Task<DetalleRevisionResponseDTO?> ObtenerPorIdAsync(string claveRevision, string claveDocAspirante)
        {
            if (string.IsNullOrWhiteSpace(claveRevision) || string.IsNullOrWhiteSpace(claveDocAspirante))
                throw new ArgumentException("Las claves de revisión y documento son obligatorias.");

            var detalle = await _repository.ObtenerPorIdAsync(claveRevision, claveDocAspirante);
            if (detalle == null)
                return null;

            return MapearResponse(detalle);
        }

        public async Task RegistrarDetalleAsync(DetalleRevisionRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveRevision))
                throw new ArgumentException("La clave de la revisión es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.ClaveDocAspirante))
                throw new ArgumentException("La clave del documento es obligatoria.");

            var estatusValidos = new[] { "Aceptado", "Rechazado" };
            if (string.IsNullOrWhiteSpace(dto.EstatusDoc) || !estatusValidos.Contains(dto.EstatusDoc))
                throw new ArgumentException("El estatus del documento debe ser 'Aceptado' o 'Rechazado'.");

            if (dto.EstatusDoc == "Rechazado" && string.IsNullOrWhiteSpace(dto.MotivoRechazo))
                throw new ArgumentException("Debe especificar el motivo de rechazo.");

            await _repository.RegistrarDetalleAsync(dto);
        }

        public async Task<DetalleRevisionResponseDTO> ActualizarDetalleAsync(string claveRevision, string claveDocAspirante, DetalleRevisionUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(claveRevision) || string.IsNullOrWhiteSpace(claveDocAspirante))
                throw new ArgumentException("Las claves de revisión y documento son obligatorias.");

            var existe = await _repository.ObtenerPorIdAsync(claveRevision, claveDocAspirante);
            if (existe == null)
                throw new InvalidOperationException("No existe un registro previo para este documento en esta revisión.");

            var estatusValidos = new[] { "Aceptado", "Rechazado" };
            if (string.IsNullOrWhiteSpace(dto.EstatusDoc) || !estatusValidos.Contains(dto.EstatusDoc))
                throw new ArgumentException("El estatus del documento debe ser 'Aceptado' o 'Rechazado'.");

            if (dto.EstatusDoc == "Rechazado" && string.IsNullOrWhiteSpace(dto.MotivoRechazo))
                throw new ArgumentException("Debe especificar el motivo de rechazo.");

            await _repository.ActualizarDetalleAsync(claveRevision, claveDocAspirante, dto);

            var actualizado = await _repository.ObtenerPorIdAsync(claveRevision, claveDocAspirante);
            return MapearResponse(actualizado!);
        }

        private static DetalleRevisionResponseDTO MapearResponse(DetalleRevisionEntity d) =>
            new DetalleRevisionResponseDTO
            {
                ClaveRevision = d.ClaveRevision,
                ClaveDocAspirante = d.ClaveDocAspirante,
                EstatusDoc = d.EstatusDoc,
                MotivoRechazo = d.MotivoRechazo
            };
    }
}
