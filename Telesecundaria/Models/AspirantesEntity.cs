using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class AspirantesEntity
    {
        public string ClaveAspirante { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string Curp { get; set; }
        public string EscuelaProcedencia { get; set; }
        public decimal PromedioPrimaria { get; set; }
        public bool TieneDiscapacidad { get; set; } = false;
        public string? NombreEnfermedad { get; set; }
        public bool HermanoPlantel { get; set; } = false;
        public string? CurpHermano { get; set; }
        public string EstatusAspirante { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Estado { get; set; } = true;
        public string ClaveConvocatoria { get; set; }
        public string ClaveTutorAspirante { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual ConvocatoriasEntity Convocatoria { get; set; }
        [JsonIgnore]
        public virtual TutorAspiranteEntity TutorAspirante { get; set; }

        // Colecciones
        public ICollection<DocumentosAspiranteEntity> DocumentosAspirante { get; set; } = new List<DocumentosAspiranteEntity>();
        public ICollection<AdjuncionesEntity> Adjunciones { get; set; } = new List<AdjuncionesEntity>();

        // Relación 1:1
        public FilaVirtualEntity FilaVirtual { get; set; }
    }
}
