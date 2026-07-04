using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Publicaciones;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class PublicacionesService : IPublicacionesService
    {
        private readonly IPublicacionesRepository _repository;

        private static readonly string[] CategoriasPermitidas =
        {
            "Eventos Culturales", "Noticia", "Aviso", "Convocatorias", "Galería"
        };

        public PublicacionesService(IPublicacionesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PublicacionResponseDTO>> ListarPublicacionesAsync()
        {
            var publicaciones = await _repository.ListarPublicacionesAsync();
            return publicaciones.Select(p => MapearResponse(p));
        } 

        public async Task<PublicacionResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave de la publicación es obligatoria.");

            var publicacion = await _repository.ObtenerPorIdAsync(clave);
            if (publicacion == null) 
                return null;

            return MapearResponse(publicacion);
        } 

        public async Task<PublicacionResponseDTO> RegistrarPublicacionAsync(PublicacionRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Titulo))
                throw new ArgumentException("El título de la publicación es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.CuerpoContenido))
                throw new ArgumentException("El cuerpo del contenido es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Categoria))
                throw new ArgumentException("La categoría es obligatoria.");
            if (!CategoriasPermitidas.Contains(dto.Categoria))
                throw new ArgumentException($"Categoría inválida. Valores permitidos: {string.Join(", ", CategoriasPermitidas)}");
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
                throw new ArgumentException("El nombre de usuario es obligatorio.");

            var publicacionCreada = await _repository.RegistrarPublicacionAsync(dto);
            return MapearResponse(publicacionCreada);
        }

        public async Task ActualizarPublicacionAsync(string clave, PublicacionUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave es requerida.");

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException("La publicación no existe.");

            if (!CategoriasPermitidas.Contains(dto.Categoria))
                throw new ArgumentException($"Categoría inválida. Valores permitidos: {string.Join(", ", CategoriasPermitidas)}");

            var publicacion = new PublicacionesEntity
            {
                ClavePublicacion = clave,
                Titulo = dto.Titulo.Trim(),
                Subtitulo = dto.Subtitulo?.Trim(),
                CuerpoContenido = dto.CuerpoContenido.Trim(),
                Categoria = dto.Categoria.Trim(),
                ClaveImagenPrincipal = dto.ImgPrincipal?.Trim(),
                ClaveImagenSecundaria = dto.ImgSecundaria?.Trim(),
                ClaveImagenTercera = dto.ImgTercera?.Trim(),
                Destacado = dto.Destacado,
                EstatusVisible = dto.EstatusVisible,
                FechaRetiro = dto.FechaRetiro
            };

            await _repository.ActualizarPublicacionAsync(clave, publicacion);
        } 

        public async Task EliminarPublicacionAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("Se requiere una clave válida.");

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException("La publicación no existe.");

            await _repository.EliminarPublicacionAsync(clave);
        }

        private static PublicacionResponseDTO MapearResponse(PublicacionesEntity p) =>
            new PublicacionResponseDTO
            {
                ClavePublicacion = p.ClavePublicacion,
                Titulo = p.Titulo,
                Subtitulo = p.Subtitulo,
                CuerpoContenido = p.CuerpoContenido,
                Categoria = p.Categoria,
                FechaAparicion = p.FechaAparicion,
                FechaRetiro = p.FechaRetiro,
                ClaveUsuario = p.ClaveUsuario,
                ClaveConvocatoria = p.ClaveConvocatoria,
                ClaveImagenPrincipal = p.ClaveImagenPrincipal,
                ClaveImagenSecundaria = p.ClaveImagenSecundaria,
                ClaveImagenTercera = p.ClaveImagenTercera,
                FechaRegistro = p.FechaRegistro,
                Destacado = p.Destacado,
                EstatusVisible = p.EstatusVisible
            };
    }
}
