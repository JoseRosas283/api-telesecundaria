namespace Telesecundaria.Services.Interfaces
{
    public interface IPdfService
    {
        Task<string> GuardarPdfAsync(IFormFile file, string tipoDocumento);
    }
}
