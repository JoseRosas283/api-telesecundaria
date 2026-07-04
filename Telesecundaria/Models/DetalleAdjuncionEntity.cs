using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class DetalleAdjuncionEntity
    {
        public string ClaveAdjuncion { get; set; }
        public string ClaveDocAspirante { get; set; }
        public string EstatusDocumento { get; set; }
        public string? MotivoRechazo { get; set; }
        public DateTime? FechaEvaluacion { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual AdjuncionesEntity Adjuncion { get; set; }
        [JsonIgnore]
        public virtual DocumentosAspiranteEntity DocumentoAspirante { get; set; }
    }
}
