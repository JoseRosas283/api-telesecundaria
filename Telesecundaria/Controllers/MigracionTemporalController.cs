using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telesecundaria.Persistence;

namespace Telesecundaria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MigracionTemporalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MigracionTemporalController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("hashear-contrasenias")]
        public async Task<IActionResult> HashearContrasenias()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            int actualizados = 0;

            foreach (var u in usuarios)
            {
                if (!u.Contrasenia.StartsWith("$2"))
                {
                    u.Contrasenia = BCrypt.Net.BCrypt.HashPassword(u.Contrasenia);
                    actualizados++;
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"{actualizados} contraseñas migradas a bcrypt." });
        }
    }
}