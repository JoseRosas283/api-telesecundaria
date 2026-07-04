namespace Telesecundaria.DTOs.Aspirantes
{
    public class AspiranteResponseDTO
    {
        public string ClaveAspirante { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public string Curp { get; set; } = string.Empty;
        public string EscuelaProcedencia { get; set; } = string.Empty;
        public decimal PromedioPrimaria { get; set; }
        public bool TieneDiscapacidad { get; set; }
        public string? NombreEnfermedad { get; set; }
        public bool HermanoPlantel { get; set; }
        public string? CurpHermano { get; set; }
        public string? EstatusAspirante { get; set; }
        public string ClaveConvocatoria { get; set; } = string.Empty;
        public string ClaveTutorAspirante { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}
