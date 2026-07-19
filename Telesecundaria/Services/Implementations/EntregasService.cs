using Telesecundaria.DTOs.Entregas;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class EntregasService : IEntregasService
    {
        private readonly IEntregasRepository _repository;

        public EntregasService(IEntregasRepository repository)
        {
            _repository = repository;
        }

        public async Task<EntregaResponseDTO> InicializarEntregaAsync(EntregaRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveCita))
                throw new ArgumentException("La clave de la cita es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.ClaveUsuario))
                throw new ArgumentException("La clave del usuario es obligatoria.");

            var yaExiste = await _repository.ObtenerPorCitaAsync(dto.ClaveCita);
            if (yaExiste != null)
                throw new ArgumentException("Esta cita ya cuenta con una entrega inicializada.");

            var claveEntrega = await _repository.CrearEntregaAsync(dto.ClaveCita, dto.ClaveUsuario);

            if (string.IsNullOrWhiteSpace(claveEntrega))
                throw new Exception("No se pudo generar la clave de la entrega.");

            var creada = await _repository.ObtenerEntregaAsync(claveEntrega);

            return new EntregaResponseDTO
            {
                ClaveEntrega = claveEntrega,
                EstadoFinal = creada?.EstadoFinal ?? "pendiente",
                ClaveCita = dto.ClaveCita,
                ClaveAspirante = creada?.ClaveAspirante ?? string.Empty,
                ClaveTutorAspirante = creada?.ClaveTutorAspirante ?? string.Empty,
                ClaveUsuario = dto.ClaveUsuario,
                FechaFormalizacion = creada?.FechaFormalizacion
            };
        }
    }
}
