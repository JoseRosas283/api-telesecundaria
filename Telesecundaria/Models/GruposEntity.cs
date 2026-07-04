namespace Telesecundaria.Models
{
    public class GruposEntity
    {
        public string ClaveGrupo { get; set; }
        public string Grado { get; set; }
        public string Seccion { get; set; }
        public int CapacidadMaxima { get; set; }
        public string Generacion { get; set; }
        public bool? Estado { get; set; }

        // Colecciones
        public ICollection<AsignacionGrupoEntity> AsignacionGrupos { get; set; } = new List<AsignacionGrupoEntity>();
    }
}
