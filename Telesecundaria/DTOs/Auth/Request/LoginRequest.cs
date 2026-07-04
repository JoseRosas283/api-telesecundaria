namespace Telesecundaria.DTOs.Auth.Request
{
    public class LoginRequest
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contrasenia { get; set; } = string.Empty;
    }
}
