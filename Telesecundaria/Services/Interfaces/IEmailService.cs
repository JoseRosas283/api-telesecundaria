namespace Telesecundaria.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> EnviarCorreoAsync(string destino, string titulo, string mensaje);
    }
}
