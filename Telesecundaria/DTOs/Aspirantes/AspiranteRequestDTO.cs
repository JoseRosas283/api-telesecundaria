namespace Telesecundaria.DTOs.Aspirantes
{
    public class AspiranteRequestDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public string Curp { get; set; } = string.Empty;
        public string EscuelaProcedencia { get; set; } = string.Empty;
        public decimal PromedioPrimaria { get; set; }
        public string DiscapacidadTexto { get; set; } = "No tiene"; 
        public string? NombreEnfermedad { get; set; }
        public string HermanoTexto { get; set; } = "No tiene";  
        public string? CurpHermano { get; set; }
        public string ClaveTutorAspirante { get; set; } = string.Empty;
    }
}
