namespace Telesecundaria.DTOs.Usuarios.Request
{
    public class UsuarioUpdateRequest
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string? Contrasenia { get; set; }
        public string? CorreoInstitucional { get; set; }
        public bool Estado { get; set; }
    }
}
