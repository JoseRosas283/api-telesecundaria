namespace Telesecundaria.DTOs.TutorAspirante
{
    public class TutorAspiranteRequestDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public string CurpTutor { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Parentesco { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;

        // Datos obligatorios para la Dirección exigidos por el SP
        public string CalleNumero { get; set; } = string.Empty;
        public string Colonia { get; set; } = string.Empty;
        public string CodigoPostal { get; set; } = string.Empty;
        public string Municipio { get; set; } = string.Empty;
    }
}
