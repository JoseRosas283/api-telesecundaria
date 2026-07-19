using Telesecundaria.DTOs.Documentos.Request;
using Telesecundaria.DTOs.Documentos.Response;
using Telesecundaria.DTOs.DocumentosAlumnos.Request;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IDocumentosRepository _repository;
        private readonly IHttpContextAccessor _http;
        private readonly string _uploadsPath;
        private readonly string? _publicBaseUrl;

        public DocumentoService(
            IDocumentosRepository repository,
            IHttpContextAccessor http,
            IConfiguration config,
            [FromKeyedServices("uploadsPath")] string uploadsPath)
        {
            _repository = repository;
            _http = http;
            _uploadsPath = uploadsPath;
            _publicBaseUrl = config["Storage:PublicBaseUrl"];

        }

        public async Task<IEnumerable<DocumentosEntity>> GetAllAsync()
            => await _repository.GetAllAsync();

        public async Task<DocumentosEntity?> GetByIdAsync(string claveDocumento)
        {
            if (string.IsNullOrWhiteSpace(claveDocumento))
                throw new ArgumentException("La clave documento es obligatoria.");
            return await _repository.GetByIdAsync(claveDocumento);
        }

        public async Task<IEnumerable<DocumentoResponse>> GetByExpedienteAsync(string claveExpediente)
        {
            var entidades = await _repository.GetByExpedienteAsync(claveExpediente);
            return entidades.Select(e => new DocumentoResponse
            {
                ClaveDocumento = e.ClaveDocumento,
                ArchivoUrl = e.ArchivoUrl,
                Estado = e.Estado,
                FechaSubida = e.FechaSubida,
                ClaveExpediente = e.ClaveExpediente,
                ClaveTipoDocumento = e.ClaveTipoDocumento,
                NombreTipoDocumento = e.TipoDocumento?.NombreDocumento ?? ""
            });
        }

        public async Task<DocumentosCargadosResponse> UploadAsync(DocumentoCreateRequest request)
        {
            var claveUsuario = _http.HttpContext!.User.FindFirst("claveUsuario")?.Value
                ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");
            request.ClaveUsuario = claveUsuario;

            var documentos = new List<(IFormFile Archivo, string NombreTipo)>
            {
                (request.ConstanciaSituacionFiscal, "Constancia de Situación Fiscal"),
                (request.CarnetISSSTe,              "Carnet o Alta del ISSSTE"),
                (request.IneIdentificacion,         "INE o Identificación Oficial"),
            };

            if (request.TituloProfesional is not null)
                documentos.Add((request.TituloProfesional, "Título Profesional"));
            if (request.CedulaProfesional is not null)
                documentos.Add((request.CedulaProfesional, "Cédula Profesional"));

        

            var documentosRespuesta = new List<DocumentoResponse>();

            foreach (var (archivo, nombreTipo) in documentos)
            {
                Directory.CreateDirectory(_uploadsPath);
                var nombreArchivo = $"{Guid.NewGuid():N}.pdf";
                var rutaFisica = Path.Combine(_uploadsPath, nombreArchivo);

                await using var stream = new FileStream(rutaFisica, FileMode.Create);
                await archivo.CopyToAsync(stream);

                var ctx = _http.HttpContext!;
                var baseUrl = string.IsNullOrWhiteSpace(_publicBaseUrl)
                    ? $"{ctx.Request.Scheme}://{ctx.Request.Host}"
                    : _publicBaseUrl!.TrimEnd('/');

                request.ArchivoUrl = $"{baseUrl}/uploads/expedientes/{nombreArchivo}";
                request.NombreTipoDocumento = nombreTipo;

                var entidad = await _repository.CreateAsync(request);

                documentosRespuesta.Add(new DocumentoResponse
                {
                    ClaveDocumento = entidad.ClaveDocumento,
                    ArchivoUrl = entidad.ArchivoUrl,
                    Estado = entidad.Estado,
                    FechaSubida = entidad.FechaSubida,
                    ClaveExpediente = entidad.ClaveExpediente,
                    ClaveTipoDocumento = entidad.ClaveTipoDocumento,
                    NombreTipoDocumento = entidad.TipoDocumento?.NombreDocumento ?? ""
                });
            }

            return new DocumentosCargadosResponse
            {
                ClaveExpediente = request.ClaveExpediente,
                Documentos = documentosRespuesta
            };
        }

        public async Task<DocumentosCargadosResponse> UploadIntendenteAsync(DocumentoIntendenteCreateRequest request)
        {
            var claveUsuario = _http.HttpContext!.User.FindFirst("claveUsuario")?.Value
                ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");
            request.ClaveUsuario = claveUsuario;

            var documentos = new List<(IFormFile Archivo, string NombreTipo)>
    {
        (request.ConstanciaSituacionFiscal, "Constancia de Situación Fiscal"),
        (request.CarnetISSSTe,              "Carnet o Alta del ISSSTE"),
        (request.IneIdentificacion,         "INE o Identificación Oficial"),
    };

            var documentosRespuesta = new List<DocumentoResponse>();

            foreach (var (archivo, nombreTipo) in documentos)
            {
                Directory.CreateDirectory(_uploadsPath);
                var nombreArchivo = $"{Guid.NewGuid():N}.pdf";
                var rutaFisica = Path.Combine(_uploadsPath, nombreArchivo);

                await using var stream = new FileStream(rutaFisica, FileMode.Create);
                await archivo.CopyToAsync(stream);

                var ctx = _http.HttpContext!;
                var baseUrl = string.IsNullOrWhiteSpace(_publicBaseUrl)
                    ? $"{ctx.Request.Scheme}://{ctx.Request.Host}"
                    : _publicBaseUrl!.TrimEnd('/');

                request.ArchivoUrl = $"{baseUrl}/uploads/expedientes/{nombreArchivo}";
                request.NombreTipoDocumento = nombreTipo;

                var entidad = await _repository.CreateIntendenteAsync(request);

                documentosRespuesta.Add(new DocumentoResponse
                {
                    ClaveDocumento = entidad.ClaveDocumento,
                    ArchivoUrl = entidad.ArchivoUrl,
                    Estado = entidad.Estado,
                    FechaSubida = entidad.FechaSubida,
                    ClaveExpediente = entidad.ClaveExpediente,
                    ClaveTipoDocumento = entidad.ClaveTipoDocumento,
                    NombreTipoDocumento = entidad.TipoDocumento?.NombreDocumento ?? ""
                });
            }

            return new DocumentosCargadosResponse
            {
                ClaveExpediente = request.ClaveExpediente,
                Documentos = documentosRespuesta
            };
        }



        public async Task<DocumentosCargadosResponse> UploadAlumnosAsync(DocumentoAlumnoCreateRequest request)
        {
            var claveUsuario = _http.HttpContext!.User.FindFirst("claveUsuario")?.Value
                ?? throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");
            request.ClaveUsuario = claveUsuario;

            var documentos = new List<(IFormFile Archivo, string NombreTipo)>
            {
                (request.CertificadoPrimaria, "Certificado de Primaria"),
                (request.CartaBuenaConducta,  "Carta de Buena Conducta"),
            };

            var documentosRespuesta = new List<DocumentoResponse>();

            foreach (var (archivo, nombreTipo) in documentos)
            {
                Directory.CreateDirectory(_uploadsPath);
                var nombreArchivo = $"{Guid.NewGuid():N}.pdf";
                var rutaFisica = Path.Combine(_uploadsPath, nombreArchivo);

                await using var stream = new FileStream(rutaFisica, FileMode.Create);
                await archivo.CopyToAsync(stream);

                var ctx = _http.HttpContext!;
                var baseUrl = string.IsNullOrWhiteSpace(_publicBaseUrl)
                    ? $"{ctx.Request.Scheme}://{ctx.Request.Host}"
                    : _publicBaseUrl!.TrimEnd('/');

                request.ArchivoUrl = $"{baseUrl}/uploads/expedientes/{nombreArchivo}";
                request.NombreTipoDocumento = nombreTipo;

                var entidad = await _repository.CreateAlumnosAsync(request);

                documentosRespuesta.Add(new DocumentoResponse
                {
                    ClaveDocumento = entidad.ClaveDocumento,
                    ArchivoUrl = entidad.ArchivoUrl,
                    Estado = entidad.Estado,
                    FechaSubida = entidad.FechaSubida,
                    ClaveExpediente = entidad.ClaveExpediente,
                    ClaveTipoDocumento = entidad.ClaveTipoDocumento,
                    NombreTipoDocumento = entidad.TipoDocumento?.NombreDocumento ?? ""
                });
            }

            return new DocumentosCargadosResponse
            {
                ClaveExpediente = request.ClaveExpediente,
                Documentos = documentosRespuesta
            };
        }
    }
}
