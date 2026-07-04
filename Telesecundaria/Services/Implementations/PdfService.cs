using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class PdfService : IPdfService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PdfService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GuardarPdfAsync(IFormFile file, string tipoDocumento)
        {
            // Convierte el nombre del documento "ACTA DE NACIMIENTO" a un tipo para carpetas "acta-de-nacimiento"
            var slug = tipoDocumento.ToLower().Trim().Replace(" ", "-");

            var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pdfs", slug);
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

            // Genera un nombre único por archivo
            var nombreArchivo = $"{Guid.NewGuid()}.pdf";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            // Guarda el archivo fisicamente
            using var stream = new FileStream(rutaFisica, FileMode.Create);
            await file.CopyToAsync(stream);

            // Contruye la url 
            var request = _httpContextAccessor.HttpContext!.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            // URL de ejemplo http://host/uploads/pdfs/acta-de-nacimiento/guid.pdf
            return $"{baseUrl}/uploads/pdfs/{slug}/{nombreArchivo}";
        }
    }
}
