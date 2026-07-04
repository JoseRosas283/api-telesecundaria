using Telesecundaria.DTOs.Receptores;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class ReceptoresService : IReceptoresService
    {
        private readonly IReceptoresRepository _repository;

        private static readonly string[] TiposReceptorValidos = new[] { "TutorAspirante", "Tutor", "Usuario" };

        public ReceptoresService(IReceptoresRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ReceptorResponseDTO>> ListarAsync()
        {
            var receptores = await _repository.ListarAsync();
            return receptores.Select(r => MapearResponse(r));
        }

        public async Task<ReceptorResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del receptor es obligatoria.");

            var receptor = await _repository.ObtenerPorIdAsync(clave);
            if (receptor == null)
                return null;

            return MapearResponse(receptor);
        }

        public async Task<ReceptorResponseDTO> RegistrarAsync(ReceptorRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TipoReceptor) || !TiposReceptorValidos.Contains(dto.TipoReceptor))
                throw new ArgumentException("El tipo de receptor debe ser 'TutorAspirante', 'Tutor' o 'Usuario'.");
            if (string.IsNullOrWhiteSpace(dto.ClaveReferencia))
                throw new ArgumentException("La clave de referencia es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.Correo))
                throw new ArgumentException("El correo es obligatorio.");

            await _repository.RegistrarAsync(dto);

            // Buscamos el último receptor creado para esa referencia
            var receptores = await _repository.ListarAsync();
            var creado = receptores
                .Where(r => (r.ClaveTutorAspirante == dto.ClaveReferencia
                          || r.ClaveTutor == dto.ClaveReferencia
                          || r.ClaveUsuario == dto.ClaveReferencia)
                          && r.TipoReceptor == dto.TipoReceptor)
                .OrderByDescending(r => r.ClaveReceptor)
                .FirstOrDefault();

            return MapearResponse(creado!);
        }

        public async Task<ReceptorResponseDTO> ActualizarAsync(string claveReceptor, ReceptorUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(claveReceptor))
                throw new ArgumentException("La clave del receptor es obligatoria.");

            var existe = await _repository.ObtenerPorIdAsync(claveReceptor);
            if (existe == null)
                throw new InvalidOperationException($"El receptor con clave {claveReceptor} no existe.");

            if (string.IsNullOrWhiteSpace(dto.NuevoCorreo))
                throw new ArgumentException("El nuevo correo es obligatorio.");

            await _repository.ActualizarAsync(claveReceptor, dto);

            var actualizado = await _repository.ObtenerPorIdAsync(claveReceptor);
            return MapearResponse(actualizado!);
        }

        public async Task EliminarAsync(string claveReceptor)
        {
            if (string.IsNullOrWhiteSpace(claveReceptor))
                throw new ArgumentException("La clave del receptor es obligatoria.");

            var existe = await _repository.ObtenerPorIdAsync(claveReceptor);
            if (existe == null)
                throw new InvalidOperationException($"El receptor con clave {claveReceptor} no existe.");

            await _repository.EliminarAsync(claveReceptor);
        }

        private static ReceptorResponseDTO MapearResponse(ReceptoresEntity r) =>
            new ReceptorResponseDTO
            {
                ClaveReceptor = r.ClaveReceptor,
                TipoReceptor = r.TipoReceptor,
                ClaveTutorAspirante = r.ClaveTutorAspirante,
                ClaveTutor = r.ClaveTutor,
                ClaveUsuario = r.ClaveUsuario,
                CorreoDestino = r.CorreoDestino,
                Estado = r.Estado
            };
    }
}
