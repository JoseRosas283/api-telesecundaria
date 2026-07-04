using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class InscripcionesEntity
    {
        public string ClaveInscripcion { get; set; }
        public string ClaveAlumno { get; set; }
        public string ClaveCiclo { get; set; }
        public string ClavePeriodo { get; set; }
        public string? ClaveGrupo { get; set; }
        public string ClaveUsuario { get; set; }
        public string? ClavePago { get; set; }
        public DateTime? FechaInscripcion { get; set; }
        public string? EstatusInscripcion { get; set; }
        public string? Observaciones { get; set; }

        // Navegación
        [JsonIgnore]
        public virtual AlumnosEntity Alumno { get; set; }
        [JsonIgnore]
        public virtual CiclosEscolaresEntity CicloEscolar { get; set; }
        [JsonIgnore]
        public virtual PeriodosEntity Periodo { get; set; }
        [JsonIgnore]
        public virtual GruposEntity? Grupo { get; set; }
        [JsonIgnore]
        public virtual UsuariosEntity Usuario { get; set; }
        [JsonIgnore]
        public virtual PagosEntity? Pago { get; set; }
    }
}
