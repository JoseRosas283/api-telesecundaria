using Telesecundaria.DTOs.DetalleRevision;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IDetalleRevisionRepository
    {
        Task<IEnumerable<DetalleRevisionEntity>> ListarPorRevisionAsync(string claveRevision);
        Task<DetalleRevisionEntity?> ObtenerPorIdAsync(string claveRevision, string claveDocAspirante);
        Task RegistrarDetalleAsync(DetalleRevisionRequestDTO dto);
        Task ActualizarDetalleAsync(string claveRevision, string claveDocAspirante, DetalleRevisionUpdateDTO dto);
    }
}
