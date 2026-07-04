using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class RevisionesEntity
    {
        public string ClaveRevision { get; set; }
        public string EstatusRevision { get; set; }
        public string EstadoOperativo { get; set; } = "Abierta";
        public string ObservacionGeneral { get; set; }
        public DateTime FechaRevision { get; set; }
        public string ClaveAdjuncion { get; set; }
        public string ClaveUsuario { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual AdjuncionesEntity Adjuncion { get; set; }
        [JsonIgnore]
        public virtual UsuariosEntity Usuario { get; set; }

        // Colecciones
        public ICollection<DetalleRevisionEntity> DetalleRevisiones { get; set; } = new List<DetalleRevisionEntity>();

        // Relaciones 1:1
        public RevisionesAceptadasEntity RevisionAceptada { get; set; }
        public CitasInscripcionEntity CitaInscripcion { get; set; }
    }
}
