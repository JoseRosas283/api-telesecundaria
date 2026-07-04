using Microsoft.EntityFrameworkCore;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class EnviosRepository : IEnviosRepository
    {
        private readonly ApplicationDbContext _context;

        public EnviosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EnviosEntity>> ListarTodosAsync()
        {
            return await _context.Envios.ToListAsync();
        }

        public async Task<IEnumerable<EnviosEntity>> ListarPendientesAsync()
        {
            return await _context.Envios
                .Include(e => e.Notificacion)
                .Where(e => e.Estatus == "Pendiente")
                .ToListAsync();
        }

        public async Task<EnviosEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Envios
                .FirstOrDefaultAsync(e => e.ClaveEnvio == clave);
        }

        public async Task MarcarComoEnviadoAsync(string claveEnvio)
        {
            var envio = await _context.Envios.FirstOrDefaultAsync(e => e.ClaveEnvio == claveEnvio);
            if (envio != null)
            {
                envio.Estatus = "Enviado";
                envio.FechaEnvio = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarcarComoFallidoAsync(string claveEnvio, string errorLog)
        {
            var envio = await _context.Envios.FirstOrDefaultAsync(e => e.ClaveEnvio == claveEnvio);
            if (envio != null)
            {
                envio.Estatus = "Fallido";
                envio.ReintentoNum = envio.ReintentoNum + 1;
                envio.ErrorLog = errorLog;
                await _context.SaveChangesAsync();
            }
        }
    }
}
