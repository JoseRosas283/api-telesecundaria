namespace Telesecundaria.DTOs.Grupos
{
    public class GrupoResponseDTO
    {
        public string ClaveGrupo { get; set; } = string.Empty;
        public string Grado { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public int CapacidadMaxima { get; set; }
        public string Generacion { get; set; } = string.Empty;
        public bool? Estado { get; set; }
    }
}
