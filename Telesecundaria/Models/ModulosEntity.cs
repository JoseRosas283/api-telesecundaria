using System.Text.Json.Serialization;

namespace Telesecundaria.Models
{
    public class ModulosEntity
    {
        public string ClaveModulo { get; set; }
        public string NombreModulo { get; set; }
        public string? Descripcion { get; set; }
        public string? UrlModulo { get; set; }
        public bool EstadoModulo { get; set; } = true;

        // Autorreferencia: módulo padre (null = es raíz)
        public string? ClaveModuloPadre { get; set; }

        // Navegación interna
        [JsonIgnore]
        public ModulosEntity? ModuloPadre { get; set; }
        public ICollection<ModulosEntity> SubModulos { get; set; } = new List<ModulosEntity>();

        // Navegación hacia permisos
        [JsonIgnore]
        public ICollection<PermisosEntity> Permisos { get; set; } = new List<PermisosEntity>();
    }
}
