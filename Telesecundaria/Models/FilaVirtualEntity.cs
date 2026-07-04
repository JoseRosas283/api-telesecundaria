using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class FilaVirtualEntity
    {
        public string ClaveFila { get; set; }
        public string ClaveConvocatoria { get; set; }
        public string ClaveAspirante { get; set; }
        public int NumeroLugar { get; set; }
        public DateTime FechaAsignacion { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual ConvocatoriasEntity Convocatoria { get; set; }
        [JsonIgnore]
        public virtual AspirantesEntity Aspirante { get; set; }
    }
}
