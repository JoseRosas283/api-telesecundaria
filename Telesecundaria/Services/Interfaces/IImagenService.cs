namespace Telesecundaria.Services.Interfaces
{
    public interface IImagenService
    {
        Task<string> GuardarImagenAsync(IFormFile file);
    }
}
