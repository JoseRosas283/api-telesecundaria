namespace Telesecundaria.DTOs.Convocatorias
{
    public class ConvocatoriaResponseDTO
    {
        public string ClaveConvocatoria { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string CicloEscolar { get; set; } = string.Empty;
        public int? CupoMaximo { get; set; }
        public int? CupoDisponible { get; set; }
        public bool Activacion { get; set; }
        public string? Estado { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
