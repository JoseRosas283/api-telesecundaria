using Telesecundaria.Models;

namespace Telesecundaria.Repositories.Interfaces
{
    public interface IAdjuncionesRepository
    {
        Task<string> CrearAdjuncionAsync(string claveTutor, string claveAspirante);
        Task<string> InsertarDocumentoAspirante(string rutaArchivo, string claveAspirante, string nombreTipoDocumento);
        Task InsertarDetalleAdjuncionAsync(string claveAdjuncion, string claveDocAspirante);
        Task<AdjuncionesEntity?> ObtenerAdjuncionAsync(string claveAdjuncion);
        Task<List<DocumentosAspiranteEntity>> ObtenerDocumentosAdjuncionAsync(string claveAdjuncion);

        Task<List<DocumentosAspiranteEntity>> ObtenerDocumentosPorAspiranteAsync(string claveAspirante);
        Task<List<DocumentosAspiranteEntity>> ObtenerDocumentosSinDetalleAsync(string claveAspirante);

        Task<List<DocumentoConEstatusProjection>> ObtenerDocumentosConEstatusAsync(string claveAspirante);
        Task ActualizarRutaDocumentoRechazadoAsync(string claveDocAspirante, string nuevaRuta);

        Task<List<DocumentosAspiranteEntity>> ObtenerDocumentosRechazadosAsync(string claveAspirante);
    }
}
