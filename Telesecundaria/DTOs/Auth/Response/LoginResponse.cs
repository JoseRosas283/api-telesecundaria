namespace Telesecundaria.DTOs.Auth.Response
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string ClaveUsuario { get; set; } = string.Empty;
        public string ClaveLogueo { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonIgnore]
        public string RefreshToken { get; set; } = string.Empty;


    }
}
