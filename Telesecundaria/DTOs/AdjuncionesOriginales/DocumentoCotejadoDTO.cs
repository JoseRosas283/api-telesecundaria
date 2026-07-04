namespace Telesecundaria.DTOs.AdjuncionesOriginales
{
    public class ProgresoAdjuncionOriginalDTO
    {
        public string ClaveAdjOriginal { get; set; } = string.Empty;
        public int DocumentosRegistrados { get; set; }
        public int DocumentosRequeridos { get; set; }
        public bool Completado { get; set; }
        public List<DetalleAdjuncionOriginalResponseDTO> Documentos { get; set; } = new();
    }
}
