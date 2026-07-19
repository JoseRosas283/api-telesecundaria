namespace Telesecundaria.DTOs.AuthTutor.Request
{
    public class ConfirmarCambioContrasenaRequest
    {
        public string Correo { get; set; } = string.Empty;
        public string TokenConfirmacion { get; set; } = string.Empty;
        public string NuevaContrasena { get; set; } = string.Empty;

    }
}
