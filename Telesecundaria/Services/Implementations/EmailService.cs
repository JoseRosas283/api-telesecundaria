using MailKit.Net.Smtp;
using MimeKit;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> EnviarCorreoAsync(string destino, string titulo, string mensaje)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config["Smtp:From"]));
                email.To.Add(MailboxAddress.Parse(destino));
                email.Subject = titulo;
                email.Body = new TextPart("plain") { Text = mensaje };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _config["Smtp:Host"],
                    int.Parse(_config["Smtp:Port"]!),
                    MailKit.Security.SecureSocketOptions.StartTls
                );
                await smtp.AuthenticateAsync(_config["Smtp:User"], _config["Smtp:Password"]);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[EmailService] Falló el envío: {ex.GetType().Name} - {ex.Message}");
                return false;
            }
        }
    }
}
