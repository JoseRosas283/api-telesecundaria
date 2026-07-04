using Telesecundaria.DTOs.AdjuncionesOriginales;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class AdjuncionesOriginalesService : IAdjuncionesOriginalesService
    {
        private readonly IAdjuncionesOriginalesRepository _repository;

        public AdjuncionesOriginalesService(IAdjuncionesOriginalesRepository repository)
        {
            _repository = repository;
        }

        public async Task<AdjuncionOriginalResponseDTO> RegistrarAdjuncionOriginalAsync(AdjuncionOriginalRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveEntrega))
                throw new ArgumentException("La clave de la entrega es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.ClaveUsuario))
                throw new ArgumentException("La clave del usuario es obligatoria.");

            var yaExiste = await _repository.ObtenerPorEntregaAsync(dto.ClaveEntrega);
            if (yaExiste != null)
                throw new ArgumentException("Esta entrega ya cuenta con un expediente digital original registrado.");

            var claveAdjOriginal = await _repository.CrearAdjuncionOriginalAsync(dto.ClaveEntrega, dto.ClaveUsuario);

            if (string.IsNullOrWhiteSpace(claveAdjOriginal))
                throw new Exception("No se pudo generar la clave de la adjunción original.");

            var creada = await _repository.ObtenerAdjuncionOriginalAsync(claveAdjOriginal);

            return new AdjuncionOriginalResponseDTO
            {
                ClaveAdjOriginal = claveAdjOriginal,
                ClaveEntrega = dto.ClaveEntrega,
                ClaveUsuario = dto.ClaveUsuario,
                FechaCarga = creada?.FechaCarga
            };
        }

        public async Task<DetalleAdjuncionOriginalResponseDTO> RegistrarDetalleAsync(DetalleAdjuncionOriginalRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveAdjOriginal))
                throw new ArgumentException("La clave de la adjunción original es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.ClaveDocAspirante))
                throw new ArgumentException("La clave del documento es obligatoria.");

            await _repository.InsertarDetalleAdjuncionOriginalAsync(dto.ClaveAdjOriginal, dto.ClaveDocAspirante);

            var detalles = await _repository.ObtenerDetallesAsync(dto.ClaveAdjOriginal);
            var ultimo = detalles.FirstOrDefault(d => d.ClaveDocAspirante == dto.ClaveDocAspirante);

            return new DetalleAdjuncionOriginalResponseDTO
            {
                ClaveAdjOriginal = dto.ClaveAdjOriginal,
                ClaveDocAspirante = dto.ClaveDocAspirante,
                RutaPdfOriginal = ultimo?.RutaPdfOriginal ?? string.Empty
            };
        }

        public async Task<ProgresoAdjuncionOriginalDTO> ObtenerProgresoAsync(string claveAdjOriginal)
        {
            if (string.IsNullOrWhiteSpace(claveAdjOriginal))
                throw new ArgumentException("La clave de la adjunción original es obligatoria.");

            var detalles = await _repository.ObtenerDetallesAsync(claveAdjOriginal);
            var totalRequerido = await _repository.ContarRequisitosInscripcionAsync();

            var adjuncion = await _repository.ObtenerAdjuncionOriginalAsync(claveAdjOriginal);
            var estadoEntrega = adjuncion != null
                ? await _repository.ObtenerEstadoEntregaAsync(adjuncion.ClaveEntrega)
                : null;

            return new ProgresoAdjuncionOriginalDTO
            {
                ClaveAdjOriginal = claveAdjOriginal,
                DocumentosRegistrados = detalles.Count,
                DocumentosRequeridos = totalRequerido,
                Completado = string.Equals(estadoEntrega, "Completada", StringComparison.OrdinalIgnoreCase),
                Documentos = detalles.Select(d => new DetalleAdjuncionOriginalResponseDTO
                {
                    ClaveAdjOriginal = d.ClaveAdjOriginal,
                    ClaveDocAspirante = d.ClaveDocAspirante,
                    RutaPdfOriginal = d.RutaPdfOriginal
                }).ToList()
            };
        }
    }
}
