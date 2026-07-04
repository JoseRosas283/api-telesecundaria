namespace Telesecundaria.DTOs.Receptores
{
    public class ReceptorResponseDTO
    {
        public string ClaveReceptor { get; set; } = string.Empty;
        public string TipoReceptor { get; set; } = string.Empty;
        public string? ClaveTutorAspirante { get; set; }
        public string? ClaveTutor { get; set; }
        public string? ClaveUsuario { get; set; }
        public string CorreoDestino { get; set; } = string.Empty;
        public bool? Estado { get; set; }
    }
}
