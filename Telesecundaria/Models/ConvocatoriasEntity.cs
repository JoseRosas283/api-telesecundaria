namespace Telesecundaria.Models
{
    public class ConvocatoriasEntity
    {
        public string ClaveConvocatoria { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string CicloEscolar { get; set; }
        public int? CupoMaximo { get; set; }
        public int? CupoDisponible { get; set; }
        public bool Activacion { get; set; } = false;
        public string Estado { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Colecciones
        public ICollection<AspirantesEntity> Aspirantes { get; set; } = new List<AspirantesEntity>();
        public ICollection<FilaVirtualEntity> FilaVirtual { get; set; } = new List<FilaVirtualEntity>();
        public ICollection<PublicacionesEntity> Publicaciones { get; set; } = new List<PublicacionesEntity>();
    }
}
