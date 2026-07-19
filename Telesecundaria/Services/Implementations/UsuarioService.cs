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

            request.Contrasenia = BCrypt.Net.BCrypt.HashPassword(request.Contrasenia);

            return await _repository.CreateAsync(request);


        }

        public async Task UpdateAsync(string claveUsuario, UsuarioUpdateRequest request)
        {

            var existente = await _repository.GetByIdAsync(claveUsuario);
            if (existente == null)
                throw new KeyNotFoundException($"No se encontró el usuario con clave: {claveUsuario}");

            request.Contrasenia = BCrypt.Net.BCrypt.HashPassword(request.Contrasenia);

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
