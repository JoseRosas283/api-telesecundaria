using Telesecundaria.DTOs;
using Telesecundaria.DTOs.TutorAspirante;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class TutorAspiranteService : ITutorAspiranteService
    {
        private readonly ITutorAspiranteRepository _repository;

        public TutorAspiranteService(ITutorAspiranteRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TutorAspiranteResponseDTO>> ListarTutoresAsync()
        {
            var tutores = await _repository.ListarTutoresAsync();
            return tutores.Select(t => MapearResponse(t));
        }

        public async Task<TutorAspiranteResponseDTO?> ObtenerPorIdAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del tutor aspirante es obligatoria.");

            var tutor = await _repository.ObtenerPorIdAsync(clave);
            if (tutor == null)
                return null;

            return MapearResponse(tutor);
        }

        public async Task<TutorAspiranteResponseDTO?> ObtenerPorCurpAsync(string curp)
        {
            if (string.IsNullOrWhiteSpace(curp))
                throw new ArgumentException("El CURP es obligatorio.");

            var tutor = await _repository.ObtenerPorCurpAsync(curp.Trim().ToUpper());
            if (tutor == null)
                return null;

            return MapearResponse(tutor);
        }

        public async Task<TutorAspiranteResponseDTO> RegistrarTutorAsync(TutorAspiranteRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del tutor es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno))
                throw new ArgumentException("El apellido paterno es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.CurpTutor) || dto.CurpTutor.Length != 18)
                throw new ArgumentException("El CURP debe tener exactamente 18 caracteres.");
            if (string.IsNullOrWhiteSpace(dto.Telefono))
                throw new ArgumentException("El teléfono es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Correo))
                throw new ArgumentException("El correo es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Parentesco))
                throw new ArgumentException("El parentesco es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.Contrasena)) 
                throw new ArgumentException("La contraseña es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.CalleNumero) || string.IsNullOrWhiteSpace(dto.Colonia) || string.IsNullOrWhiteSpace(dto.CodigoPostal) || string.IsNullOrWhiteSpace(dto.Municipio))
                throw new ArgumentException("Todos los campos de la dirección son requeridos.");

            dto.Nombre = dto.Nombre.Trim();
            dto.ApellidoPaterno = dto.ApellidoPaterno.Trim();
            dto.ApellidoMaterno = dto.ApellidoMaterno?.Trim();
            dto.CurpTutor = dto.CurpTutor.Trim().ToUpper();
            dto.Telefono = dto.Telefono.Trim();
            dto.Correo = dto.Correo.Trim().ToLower();
            dto.CalleNumero = dto.CalleNumero.Trim();
            dto.Colonia = dto.Colonia.Trim();
            dto.CodigoPostal = dto.CodigoPostal.Trim();
            dto.Municipio = dto.Municipio.Trim();

            // Validar CURP único
            var curpExistente = await _repository.ObtenerPorCurpAsync(dto.CurpTutor.Trim().ToUpper());
            if (curpExistente != null)
                throw new InvalidOperationException($"Ya existe un tutor registrado con el CURP: {dto.CurpTutor}");

            dto.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);


            var tutorCreado = await _repository.RegistrarTutorAsync(dto);
            return MapearResponse(tutorCreado);
        }

        public async Task ActualizarTutorAsync(string clave, TutorAspiranteUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave del tutor es requerida.");

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException("El tutor aspirante no existe.");

            if (string.IsNullOrWhiteSpace(dto.Nombre)) 
                throw new ArgumentException("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.ApellidoPaterno)) 
                throw new ArgumentException("El apellido paterno es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.CurpTutor) || dto.CurpTutor.Length != 18) 
                throw new ArgumentException("La CURP debe tener 18 caracteres.");
            if (string.IsNullOrWhiteSpace(dto.Contrasena)) 
                throw new ArgumentException("La contraseña es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.CalleNumero)) 
                throw new ArgumentException("La calle y número son obligatorios.");
            if (string.IsNullOrWhiteSpace(dto.Colonia)) 
                throw new ArgumentException("La colonia es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.Municipio)) 
                throw new ArgumentException("El municipio es obligatorio.");
            if (string.IsNullOrWhiteSpace(dto.CodigoPostal)) 
                throw new ArgumentException("El código postal es obligatorio.");
          
            dto.Nombre = dto.Nombre.Trim();
            dto.ApellidoPaterno = dto.ApellidoPaterno.Trim();
            dto.ApellidoMaterno = dto.ApellidoMaterno?.Trim();
            dto.CurpTutor = dto.CurpTutor.Trim().ToUpper();
            dto.Telefono = dto.Telefono.Trim();
            dto.Correo = dto.Correo.Trim().ToLower();
            dto.CalleNumero = dto.CalleNumero.Trim();
            dto.Colonia = dto.Colonia.Trim();
            dto.CodigoPostal = dto.CodigoPostal.Trim();
            dto.Municipio = dto.Municipio.Trim();

            dto.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            await _repository.ActualizarTutorAsync(clave, dto);
        }

        public async Task EliminarTutorAsync(string clave)
        {
            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("Se requiere una clave válida.");

            var existe = await _repository.ObtenerPorIdAsync(clave);
            if (existe == null)
                throw new InvalidOperationException("El tutor aspirante no existe.");

            await _repository.EliminarTutorAsync(clave);
        }

        private static TutorAspiranteResponseDTO MapearResponse(TutorAspiranteEntity t) =>
        new TutorAspiranteResponseDTO
        {
            ClaveTutorAspirante = t.ClaveTutorAspirante,
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
