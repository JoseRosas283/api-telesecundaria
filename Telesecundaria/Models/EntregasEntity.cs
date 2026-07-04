using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class EntregasEntity
    {
        public string ClaveEntrega { get; set; }
        public DateTime FechaFormalizacion { get; set; }
        public string EstadoFinal { get; set; }
        public string ClaveCita { get; set; }
        public string ClaveTutorAspirante { get; set; }
        public string ClaveAspirante { get; set; }
        public string ClaveUsuario { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual CitasInscripcionEntity CitaInscripcion { get; set; }
        [JsonIgnore]
        public virtual TutorAspiranteEntity TutorAspirante { get; set; }
        [JsonIgnore]
        public virtual AspirantesEntity Aspirante { get; set; }
        [JsonIgnore]
        public virtual UsuariosEntity Usuario { get; set; }

        // Colecciones
        public ICollection<ValidacionDocumentosEntity> ValidacionDocumentos { get; set; } = new List<ValidacionDocumentosEntity>();

        // Relación 1:1
        public AdjuncionesOriginalesEntity AdjuncionOriginal { get; set; }
    }
}
