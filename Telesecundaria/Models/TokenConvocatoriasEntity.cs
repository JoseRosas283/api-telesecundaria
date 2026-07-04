namespace Telesecundaria.Models
{
    public class TokenConvocatoriasEntity
    {
        public string ClaveTokenConvocatoria { get; set; }
        public string TokenOriginal { get; set; }

        public string ClaveTutorAspirante { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public string? IpOrigen { get; set; }
        public string? DispositivoOrigen { get; set; }
        public bool EstadoSesion { get; set; } = true;

        // Navegación
        public TutorAspiranteEntity TutorAspirante { get; set; }
    }
}
