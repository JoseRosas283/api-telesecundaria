using Telesecundaria.DTOs.DestinoNotificacion;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class DestinoNotificacionService : IDestinoNotificacionService
    {
        private readonly IDestinoNotificacionRepository _repository;

        private static readonly string[] TiposReceptorValidos = new[] { "TutorAspirante", "Tutor", "Usuario" };

        public DestinoNotificacionService(IDestinoNotificacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DestinoNotificacionResponseDTO>> ListarAsync()
        {
            var destinos = await _repository.ListarAsync();
            return destinos.Select(d => MapearResponse(d));
        }

        public async Task<DestinoNotificacionResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del destino es obligatoria.");

            var destino = await _repository.ObtenerPorIdAsync(clave);
            if (destino == null)
                return null;

            return MapearResponse(destino);
        }

        public async Task<DestinoNotificacionResponseDTO> RegistrarAsync(DestinoNotificacionRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreProceso))
                throw new ArgumentException("El nombre del proceso es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.TipoReceptor) || !TiposReceptorValidos.Contains(dto.TipoReceptor))
                throw new ArgumentException("El tipo de receptor debe ser 'TutorAspirante', 'Tutor' o 'Usuario'.");

            await _repository.RegistrarAsync(dto);

            var creado = await _repository.ObtenerUltimoPorProcesoYReceptorAsync(dto.NombreProceso, dto.TipoReceptor);
            return MapearResponse(creado!);
        }

        private static DestinoNotificacionResponseDTO MapearResponse(DestinoNotificacionEntity d) =>
            new DestinoNotificacionResponseDTO
            {
                ClaveDestino = d.ClaveDestino,
                ClaveTipoNotificacion = d.ClaveTipoNotificacion,
                TipoReceptor = d.TipoReceptor
            };
    }
}
