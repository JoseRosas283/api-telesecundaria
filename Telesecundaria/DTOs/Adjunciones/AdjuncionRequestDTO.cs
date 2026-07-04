using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.Adjunciones
{
    public class AdjuncionRequestDTO
    {
        [Required] public string ClaveTutor { get; set; } = string.Empty;
        [Required] public string ClaveAspirante { get; set; } = string.Empty;
        [Required] public IFormFile ActaNacimiento { get; set; } = null!;
        [Required] public IFormFile Curp { get; set; } = null!;
        [Required] public IFormFile ComprobanteDomicilio { get; set; } = null!;
        [Required] public IFormFile CertificadoPrimaria { get; set; } = null!;
        [Required] public IFormFile ConstanciaEstudios { get; set; } = null!;
    }
}
