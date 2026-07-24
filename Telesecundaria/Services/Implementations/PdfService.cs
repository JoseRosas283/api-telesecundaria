using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class PdfService : IPdfService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _uploadsPath;
        private readonly string? _publicBaseUrl;

        public PdfService(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration config,
            [FromKeyedServices("uploadsPath")] string uploadsPath)
        {
            _httpContextAccessor = httpContextAccessor;
            _uploadsPath = uploadsPath;
            _publicBaseUrl = config["Storage:PublicBaseUrl"];
        }

        public async Task<string> GuardarPdfAsync(IFormFile file, string tipoDocumento)
        {
            // Convierte el nombre del documento "ACTA DE NACIMIENTO" a un tipo para carpetas "acta-de-nacimiento"
            var slug = tipoDocumento.ToLower().Trim().Replace(" ", "-");
            var carpeta = Path.Combine(_uploadsPath, "pdfs", slug);
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

            // Genera un nombre único por archivo
            var nombreArchivo = $"{Guid.NewGuid()}.pdf";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            // Guarda el archivo físicamente
            using var stream = new FileStream(rutaFisica, FileMode.Create);
            await file.CopyToAsync(stream);

            // Construye la URL
            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = string.IsNullOrWhiteSpace(_publicBaseUrl)
                ? $"{request.Scheme}://{request.Host}"
                : _publicBaseUrl!.TrimEnd('/');

            // URL de ejemplo http://host/uploads/pdfs/acta-de-nacimiento/guid.pdf
            return $"{baseUrl}/uploads/pdfs/{slug}/{nombreArchivo}";
        }
    }
}
