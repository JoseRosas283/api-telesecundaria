namespace Telesecundaria.Models
{
    public class CiclosEscolaresEntity
    {
        public string ClaveCiclo { get; set; }
        public string NombreCiclo { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public bool? Estatus { get; set; }
    }
}
