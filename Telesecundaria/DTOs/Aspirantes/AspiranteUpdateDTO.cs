namespace Telesecundaria.DTOs.Aspirantes
{
    public class AspiranteUpdateDTO
    {
        public string Curp { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string? ApellidoMaterno { get; set; }
        public decimal PromedioPrimaria { get; set; }
        public string EscuelaProcedencia { get; set; } = string.Empty;
    }
}
