using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class PagosEntity
    {
        public string ClavePago { get; set; }
        public string ClaveTutor { get; set; }
        public string ClaveUsuario { get; set; }
        public string ClaveCiclo { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaPago { get; set; }
        public string MetodoPago { get; set; }
        public string? ComprobantePago { get; set; }
        public string? Referencia { get; set; }
        public bool? EstadoPago { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual TutoresEntity Tutor { get; set; }
        [JsonIgnore]
        public virtual UsuariosEntity Usuario { get; set; }
        [JsonIgnore]
        public virtual CiclosEscolaresEntity CicloEscolar { get; set; }
    }
}
