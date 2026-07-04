namespace Telesecundaria.DTOs.Usuarios.Request
{
    public class UsuarioCreateRequest
    {
        public string ClaveEmpleado { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contrasenia { get; set; } = string.Empty;
        public string? CorreoInstitucional { get; set; }
    }
}
