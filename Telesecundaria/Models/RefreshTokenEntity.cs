namespace Telesecundaria.Models
{
    public class RefreshTokenEntity
    {
        public Guid ClaveRefreshToken { get; set; }
        public string ClaveUsuario { get; set; }
        public string ClaveLogueo { get; set; }
        public string Token { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool Revocado { get; set; }

        // Navegación
        public virtual UsuariosEntity Usuario { get; set; }


    }
}
