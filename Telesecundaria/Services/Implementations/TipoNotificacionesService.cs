using Telesecundaria.DTOs.TipoNotificaciones;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class TipoNotificacionesService : ITipoNotificacionesService
    {
        private readonly ITipoNotificacionesRepository _repository;

        private static readonly string[] ProcesosValidos = new[]
        {
            "Documentos Rechazados", "Documentos Aceptados", "Cierre de Adjuncion",
            "Citas", "Inscripciones", "Institucionales", "Docencia", "Directivas", "Administrativas"
        };

        public TipoNotificacionesService(ITipoNotificacionesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TipoNotificacionResponseDTO>> ListarAsync()
        {
            var tipos = await _repository.ListarAsync();
            return tipos.Select(t => MapearResponse(t));
        }

        public async Task<TipoNotificacionResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del tipo de notificación es obligatoria.");

            var tipo = await _repository.ObtenerPorIdAsync(clave);
            if (tipo == null)
                return null;

            return MapearResponse(tipo);
        }

        public async Task<TipoNotificacionResponseDTO> RegistrarAsync(TipoNotificacionRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreProceso) || !ProcesosValidos.Contains(dto.NombreProceso))
                throw new ArgumentException($"El nombre de proceso debe ser uno de: {string.Join(", ", ProcesosValidos)}.");
            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                throw new ArgumentException("La descripción es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.Icono))
                throw new ArgumentException("El icono es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Color))
                throw new ArgumentException("El color es obligatorio.");

            await _repository.RegistrarAsync(dto);

            var creado = await _repository.ObtenerPorNombreProcesoAsync(dto.NombreProceso.Trim());
            return MapearResponse(creado!);
        }

        private static TipoNotificacionResponseDTO MapearResponse(TipoNotificacionesEntity t) =>
            new TipoNotificacionResponseDTO
            {
                ClaveTipoNotificacion = t.ClaveTipoNotificacion,
                NombreProceso = t.NombreProceso,
                Descripcion = t.Descripcion,
                Icono = t.Icono,
                Color = t.Color
            };
    }
}
