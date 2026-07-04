namespace Telesecundaria.DTOs.CiclosEscolares
{
    public class CicloEscolarResponseDTO
    {
        public string ClaveCiclo { get; set; } = string.Empty;
        public string NombreCiclo { get; set; } = string.Empty;
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public bool? Estatus { get; set; }
    }
}
