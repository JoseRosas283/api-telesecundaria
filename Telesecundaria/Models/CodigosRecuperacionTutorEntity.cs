namespace Telesecundaria.Models
{
    public class CodigosRecuperacionTutorEntity
    {
        public string ClaveCodigoRecuperacion { get; set; }
        public string ClaveTutorAspirante { get; set; }
        public string Codigo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool Usado { get; set; }
        public string? TokenConfirmacion { get; set; }
        public DateTime? TokenExpiracion { get; set; }
        public bool TokenUsado { get; set; }

        // Navegación
        public virtual TutorAspiranteEntity TutorAspirante { get; set; }

    }
}
