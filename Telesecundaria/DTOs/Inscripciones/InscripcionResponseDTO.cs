namespace Telesecundaria.DTOs.Inscripciones
{
    public class InscripcionResponseDTO
    {
        public string ClaveInscripcion { get; set; } = string.Empty;
        public string ClaveAlumno { get; set; } = string.Empty;
        public string ClaveCiclo { get; set; } = string.Empty;
        public string ClavePeriodo { get; set; } = string.Empty;
        public string? ClaveGrupo { get; set; }
        public string? ClavePago { get; set; }
        public DateTime? FechaInscripcion { get; set; }
        public string? EstatusInscripcion { get; set; }
        public string? Observaciones { get; set; }
    }
}
