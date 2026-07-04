using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class PeriodosEntity
    {
        public string ClavePeriodo { get; set; }
        public string ClaveCiclo { get; set; }
        public string NombrePeriodo { get; set; }
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }
        public bool? EstadoPeriodo { get; set; }
        public DateTime? FechaRegistro { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual CiclosEscolaresEntity CicloEscolar { get; set; }
    }
}
