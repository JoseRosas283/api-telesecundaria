using Telesecundaria.DTOs;
using Telesecundaria.DTOs.GaleriaImagenes;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class GaleriaImagenesService : IGaleriaImagenesService
    {
        private readonly IGaleriaImagenesRepository _repository;
        private readonly IImagenService _imagenService;

        private static readonly string[] TiposPermitidos =
        {
            "Eventos Culturales", "Noticia", "Aviso", "Convocatorias", "otros"
        };

        public GaleriaImagenesService(IGaleriaImagenesRepository repository, IImagenService imagenService)
        {
            _repository = repository;
            _imagenService = imagenService;
        }

        public async Task<IEnumerable<GaleriaImagenResponseDTO>> ListarImagenesAsync()
        {
            var imagenes = await _repository.ListarImagenesAsync();
            return imagenes.Select(g => MapearResponse(g));
        }

        public async Task<GaleriaImagenResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave de la imagen es obligatoria.");

            var imagen = await _repository.ObtenerPorIdAsync(clave);
            if (imagen == null) 
                return null;

            return MapearResponse(imagen);
        }

        public async Task<GaleriaImagenResponseDTO> RegistrarImagenAsync(GaleriaImagenRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreArchivo))
                throw new ArgumentException("El nombre del archivo es obligatorio.");
            if (dto.Imagen == null || dto.Imagen.Length == 0)
                throw new ArgumentException("El archivo de imagen es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.TipoRecurso))
                throw new ArgumentException("El tipo de recurso es obligatorio.");
            if (!TiposPermitidos.Contains(dto.TipoRecurso))
                throw new ArgumentException($"Tipo de recurso inválido. Valores permitidos: {string.Join(", ", TiposPermitidos)}");

            var rutaUrl = await _imagenService.GuardarImagenAsync(dto.Imagen);

            var imagen = new GaleriaImagenesEntity
            {
                NombreArchivo = dto.NombreArchivo.Trim(),
                RutaUrl = rutaUrl,
                TipoRecurso = dto.TipoRecurso.Trim()
            };

            var imagenCreada = await _repository.RegistrarImagenAsync(imagen);
            return MapearResponse(imagenCreada);
        }

        public async Task ActualizarImagenAsync(string clave, GaleriaImagenUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave es requerida.");

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException("La imagen no existe.");

            if (string.IsNullOrWhiteSpace(dto.NombreArchivo))
                throw new ArgumentException("El nombre del archivo es obligatorio.");

            if (!TiposPermitidos.Contains(dto.TipoRecurso))
                throw new ArgumentException($"Tipo inválido. Permitidos: {string.Join(", ", TiposPermitidos)}");

            string rutaUrl;
            if (dto.Imagen != null && dto.Imagen.Length > 0)
                rutaUrl = await _imagenService.GuardarImagenAsync(dto.Imagen);
            else
                rutaUrl = existe.RutaUrl;

            var imagen = new GaleriaImagenesEntity
            {
                ClaveImagen = clave,
                NombreArchivo = dto.NombreArchivo.Trim(),
                RutaUrl = rutaUrl,
                TipoRecurso = dto.TipoRecurso.Trim()
            };

            await _repository.ActualizarImagenAsync(clave, imagen);
        }

        public async Task EliminarImagenAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("Se requiere una clave válida.");

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException("La imagen no existe.");

            await _repository.EliminarImagenAsync(clave);
        }

        private static GaleriaImagenResponseDTO MapearResponse(GaleriaImagenesEntity g) =>
            new GaleriaImagenResponseDTO
            {
                ClaveImagen = g.ClaveImagen,
                NombreArchivo = g.NombreArchivo,
                RutaUrl = g.RutaUrl,
                TipoRecurso = g.TipoRecurso,
                FechaRegistro = g.FechaRegistro
            };
    }
}
