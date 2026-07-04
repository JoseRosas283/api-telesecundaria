using Telesecundaria.DTOs.Tutores;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class TutoresService : ITutoresService
    {
        private readonly ITutoresRepository _repository;

        public TutoresService(ITutoresRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TutorResponseDTO>> ListarTutoresAsync()
        {
            var tutores = await _repository.ListarTutoresAsync();
            return tutores.Select(t => MapearResponse(t));
        }

        public async Task<TutorResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del tutor es obligatoria.");

            var tutor = await _repository.ObtenerPorIdAsync(clave);
            if (tutor == null)
                return null;

            return MapearResponse(tutor);
        }

        public async Task<TutorResponseDTO> RegistrarTutorAsync(TutorRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del tutor es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                throw new ArgumentException("El apellido paterno es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.CurpTutor))
                throw new ArgumentException("La CURP del tutor es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.Telefono))
                throw new ArgumentException("El teléfono es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Correo))
                throw new ArgumentException("El correo es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Parentesco))
                throw new ArgumentException("El parentesco es obligatorio.");

            await _repository.RegistrarTutorAsync(dto);

            var creado = await _repository.ObtenerPorCurpAsync(dto.CurpTutor.ToUpper().Trim());
            return MapearResponse(creado!);
        }

        public async Task<TutorResponseDTO> ActualizarTutorAsync(string claveTutor, TutorUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(claveTutor))
                throw new ArgumentException("La clave del tutor es obligatoria.");

            var existe = await _repository.ObtenerPorIdAsync(claveTutor);
            if (existe == null)
                throw new InvalidOperationException($"El tutor con clave {claveTutor} no existe.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del tutor es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                throw new ArgumentException("El apellido paterno es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.CurpTutor))
                throw new ArgumentException("La CURP del tutor es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.Telefono))
                throw new ArgumentException("El teléfono es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Correo))
                throw new ArgumentException("El correo es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Parentesco))
                throw new ArgumentException("El parentesco es obligatorio.");

            await _repository.ActualizarTutorAsync(claveTutor, dto);

            var actualizado = await _repository.ObtenerPorIdAsync(claveTutor);
            return MapearResponse(actualizado!);
        }

        public async Task EliminarTutorAsync(string claveTutor)
        {
            if (string.IsNullOrWhiteSpace(claveTutor))
                throw new ArgumentException("La clave del tutor es obligatoria.");

            var existe = await _repository.ObtenerPorIdAsync(claveTutor);
            if (existe == null)
                throw new InvalidOperationException($"El tutor con clave {claveTutor} no existe.");

            await _repository.EliminarTutorAsync(claveTutor);
        }

        private static TutorResponseDTO MapearResponse(TutoresEntity t) =>
            new TutorResponseDTO
            {
                ClaveTutor = t.ClaveTutor,
                Nombre = t.Nombre,
                ApellidoPaterno = t.ApellidoPaterno,
                ApellidoMaterno = t.ApellidoMaterno,
                CurpTutor = t.CurpTutor,
                Telefono = t.Telefono,
                Correo = t.Correo,
                Parentesco = t.Parentesco,
                Estado = t.Estado
            };
    }
}
