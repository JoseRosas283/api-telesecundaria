namespace Telesecundaria.DTOs.TutorAspirante
{
    public class TutorAspiranteResponseDTO
    {
        public string ClaveTutorAspirante { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public string CurpTutor { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Parentesco { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}
