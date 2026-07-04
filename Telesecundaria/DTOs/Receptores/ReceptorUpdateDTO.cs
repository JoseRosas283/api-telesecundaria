using System.ComponentModel.DataAnnotations;

namespace Telesecundaria.DTOs.Receptores
{
    public class ReceptorUpdateDTO
    {
        [Required] public string NuevoCorreo { get; set; } = string.Empty;
    }
}
