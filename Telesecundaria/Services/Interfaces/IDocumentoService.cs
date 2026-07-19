using Telesecundaria.DTOs.Documentos.Request;
using Telesecundaria.DTOs.Documentos.Response;
using Telesecundaria.DTOs.DocumentosAlumnos.Request;
using Telesecundaria.Models;

namespace Telesecundaria.Services.Interfaces
{
    public interface IDocumentoService
    {
        Task<IEnumerable<DocumentosEntity>> GetAllAsync();
        Task<DocumentosEntity?> GetByIdAsync(string claveDocumento);

        Task<IEnumerable<DocumentoResponse>> GetByExpedienteAsync(string claveExpediente);
        Task<DocumentosCargadosResponse> UploadAsync(DocumentoCreateRequest request);
        Task<DocumentosCargadosResponse> UploadAlumnosAsync(DocumentoAlumnoCreateRequest request);
        Task<DocumentosCargadosResponse> UploadIntendenteAsync(DocumentoIntendenteCreateRequest request);
    }
}
