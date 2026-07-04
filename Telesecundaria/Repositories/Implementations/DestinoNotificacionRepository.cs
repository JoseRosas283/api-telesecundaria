using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.DestinoNotificacion;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class DestinoNotificacionRepository : IDestinoNotificacionRepository
    {
        private readonly ApplicationDbContext _context;

        public DestinoNotificacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DestinoNotificacionEntity>> ListarAsync()
        {
            return await _context.DestinoNotificacion.ToListAsync();
        }

        public async Task<DestinoNotificacionEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.DestinoNotificacion
                .FirstOrDefaultAsync(d => d.ClaveDestino == clave);
        }

        public async Task RegistrarAsync(DestinoNotificacionRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_configurar_destino_notificacion(
                    @p_nombre_proceso,
                    @p_tipo_receptor
                );",
                new NpgsqlParameter<string>("p_nombre_proceso", dto.NombreProceso),
                new NpgsqlParameter<string>("p_tipo_receptor", dto.TipoReceptor)
            );
        }

        public async Task<DestinoNotificacionEntity?> ObtenerUltimoPorProcesoYReceptorAsync(string nombreProceso, string tipoReceptor)
        {
            return await _context.DestinoNotificacion
                .Include(d => d.TipoNotificacion)
                .Where(d => d.TipoNotificacion.NombreProceso == nombreProceso && d.TipoReceptor == tipoReceptor)
                .OrderByDescending(d => d.ClaveDestino)
                .FirstOrDefaultAsync();
        }
    }
}
