using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class DireccionesEntity
    {
        public string ClaveDireccion { get; set; }
        public string CalleNumero { get; set; }
        public string Colonia { get; set; }
        public string CodigoPostal { get; set; }
        public string Municipio { get; set; }
        public bool EstadoVerificacion { get; set; } = true;
        public string ClaveTutorAspirante { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual TutorAspiranteEntity TutorAspirante { get; set; }
    }
}
