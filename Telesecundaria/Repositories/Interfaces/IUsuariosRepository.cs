using Telesecundaria.DTOs.Usuarios.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IUsuariosRepository
    {
        Task<IEnumerable<UsuariosEntity>> GetAllAsync();

        Task<UsuariosEntity?> GetByIdAsync(string claveUsuario);

        Task<UsuariosEntity> CreateAsync(UsuarioCreateRequest request);

        Task UpdateAsync(string claveUsuario, UsuarioUpdateRequest request);

        Task DeleteAsync(string claveUsuario);
    }
}
