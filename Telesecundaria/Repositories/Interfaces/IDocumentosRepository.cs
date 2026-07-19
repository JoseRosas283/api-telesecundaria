using Telesecundaria.DTOs.Documentos.Request;
using Telesecundaria.DTOs.DocumentosAlumnos.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IDocumentosRepository
    {
        Task<IEnumerable<DocumentosEntity>> GetAllAsync();

        Task<DocumentosEntity?> GetByIdAsync(string claveDocumento);

        Task<IEnumerable<DocumentosEntity>> GetByExpedienteAsync(string claveExpediente);

        Task<DocumentosEntity> CreateAsync(DocumentoCreateRequest request);

        Task<DocumentosEntity> CreateAlumnosAsync(DocumentoAlumnoCreateRequest request);

        Task<DocumentosEntity> CreateIntendenteAsync(DocumentoIntendenteCreateRequest request);
    }
}
