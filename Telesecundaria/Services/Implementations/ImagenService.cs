using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class ImagenService : IImagenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImagenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GuardarImagenAsync(IFormFile file)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

            var nombreSeguro = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, nombreSeguro);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            var request = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/uploads/{nombreSeguro}";
        }
    }
}
