namespace Telesecundaria.DTOs.Auth.Internal
    {
    public class UsuarioCredencialesDto
{

        public string ClaveUsuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool Estado { get; set; }


    }



}
