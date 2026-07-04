namespace Telesecundaria.DTOs.Requisitos
{
    public class RequisitoResponseDTO
    {
        public string ClaveRequisito { get; set; }
        public string EtapaProceso { get; set; } = string.Empty;
        public bool EstadoRequisito { get; set; }
        public string FormatoExigido { get; set; } = string.Empty;
        public string ClaveTipoDocumento { get; set; } = string.Empty;
    }
}
