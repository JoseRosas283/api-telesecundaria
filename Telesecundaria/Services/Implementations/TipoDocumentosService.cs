using Telesecundaria.DTOs;
using Telesecundaria.DTOs.TipoDocumentos;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class TipoDocumentosService : ITipoDocumentosService
    {
        private readonly ITipoDocumentosRepository _repository;

        public TipoDocumentosService(ITipoDocumentosRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TipoDocumentoResponseDTO>> ListarTipoDocumentosAsync()
        {
            var documentos = await _repository.ListarTipoDocumentosAsync();
            return documentos.Select(d => MapearResponse(d));
        }

        public async Task<TipoDocumentoResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del tipo de documento es obligatoria.");

            var documento = await _repository.ObtenerPorIdAsync(clave);
            if (documento == null)
                return null;

            return MapearResponse(documento);
        }

        public async Task<TipoDocumentoResponseDTO> InsertarTipoDocumentoAsync(TipoDocumentoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreDocumento))
                throw new ArgumentException("El nombre del documento es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Area))
                throw new ArgumentException("El área es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.Descripcion))
                throw new ArgumentException("La descripción es obligatoria.");

            await _repository.InsertarTipoDocumentoAsync(dto);

            // Recuperamos el recién insertado buscando por nombre normalizado
            var todos = await _repository.ListarTipoDocumentosAsync();
            var insertado = todos
                .FirstOrDefault(d => d.NombreDocumento == dto.NombreDocumento.Trim().ToUpper());

            return MapearResponse(insertado!);
        }

        private static TipoDocumentoResponseDTO MapearResponse(TipoDocumentosEntity d) =>
            new TipoDocumentoResponseDTO
            {
                ClaveTipoDocumento = d.ClaveTipoDocumento,
                NombreDocumento = d.NombreDocumento,
                Area = d.Area,
                Descripcion = d.Descripcion,
                Estado = d.Estado
            };
    }
}
