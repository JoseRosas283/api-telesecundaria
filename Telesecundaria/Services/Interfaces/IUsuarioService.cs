using Telesecundaria.DTOs.Usuarios.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuariosEntity>> GetAllAsync();

        Task<UsuariosEntity?> GetByIdAsync(string claveUsuario);

        Task<UsuariosEntity> CreateAsync(UsuarioCreateRequest resquest);

        Task UpdateAsync(string claveUsuario, UsuarioUpdateRequest request);

        Task DeleteAsync(string claveUsuario);
    }
}
