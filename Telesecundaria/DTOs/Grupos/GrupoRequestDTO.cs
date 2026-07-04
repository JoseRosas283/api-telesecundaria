namespace Telesecundaria.DTOs.Grupos
{
    public class GrupoRequestDTO
    {
        public string Grado { get; set; } = string.Empty;        // '1','2','3'
        public string Seccion { get; set; } = string.Empty;      // 'A','B'
        public int CapacidadMaxima { get; set; }
        public string Generacion { get; set; } = string.Empty;
    }
}
