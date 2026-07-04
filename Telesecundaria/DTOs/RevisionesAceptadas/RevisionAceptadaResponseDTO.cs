namespace Telesecundaria.DTOs.RevisionesAceptadas
{
    public class RevisionAceptadaResponseDTO
    {
        public string ClaveRevisionAceptada { get; set; } = string.Empty;
        public string ClaveRevision { get; set; } = string.Empty;
        public string ClaveReceptor { get; set; } = string.Empty;
        public string ClaveConvocatoria { get; set; } = string.Empty;
        public DateTime? FechaAceptacion { get; set; }
        public bool? Estado { get; set; }
    }
}
