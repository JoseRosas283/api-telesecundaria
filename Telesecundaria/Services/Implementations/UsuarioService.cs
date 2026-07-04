using Telesecundaria.DTOs.Usuarios.Request;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuariosRepository _repository;


        public UsuarioService(IUsuariosRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<UsuariosEntity>> GetAllAsync()
        {

            var usuarios = await _repository.GetAllAsync();

            return usuarios;
        }


        public async Task<UsuariosEntity?> GetByIdAsync(string claveUsuario)
        {
            if (string.IsNullOrWhiteSpace(claveUsuario))
                throw new ArgumentException("La clave del usuario no se puede estar vacia");

            var usuario = await _repository.GetByIdAsync(claveUsuario);

            if (usuario != null)
                usuario.Contrasenia = string.Empty;

            return usuario;

        }


        public async Task<UsuariosEntity> CreateAsync(UsuarioCreateRequest request)
        {

            if (string.IsNullOrWhiteSpace(request.NombreUsuario))
                throw new InvalidOperationException("Nombre del usuario es obligatorio");

            if (string.IsNullOrWhiteSpace(request.Contrasenia))
                throw new InvalidOperationException("La contrasena debe ser obligatoria");

            if (string.IsNullOrWhiteSpace(request.ClaveEmpleado))
                throw new InvalidOperationException("Un usuario debe estar ligado obligatoriamente con un empleado");

            if (!string.IsNullOrEmpty(request.CorreoInstitucional) && !request.CorreoInstitucional.Contains("@"))
                throw new InvalidOperationException("El formato del correo institucional es invalido");
            return await _repository.CreateAsync(request);


        }

        public async Task UpdateAsync(string claveUsuario, UsuarioUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(claveUsuario))
                throw new ArgumentException("La clave del usuario no puede estar vacía.");

            if (string.IsNullOrWhiteSpace(request.NombreUsuario))
                throw new InvalidOperationException("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Contrasenia))
                throw new InvalidOperationException("La contraseña es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.CorreoInstitucional) || !request.CorreoInstitucional.Contains("@"))
                throw new InvalidOperationException("El formato del correo institucional es inválido.");

            var existente = await _repository.GetByIdAsync(claveUsuario);
            if (existente == null)
                throw new KeyNotFoundException($"No se encontró el usuario con clave: {claveUsuario}");

            await _repository.UpdateAsync(claveUsuario, request);
        }

        public async Task DeleteAsync(string claveUsuario)
        {
            if (string.IsNullOrWhiteSpace(claveUsuario))
                throw new ArgumentException("La clave del usuario no puede estar vacía.");

            var usuario = await _repository.GetByIdAsync(claveUsuario);
            if (usuario == null)
                throw new KeyNotFoundException($"No se encontró el usuario con clave: {claveUsuario}");

            await _repository.DeleteAsync(claveUsuario);

        }
    }
}
