using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.TipoNotificaciones;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class TipoNotificacionesRepository : ITipoNotificacionesRepository
    {
        private readonly ApplicationDbContext _context;

        public TipoNotificacionesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoNotificacionesEntity>> ListarAsync()
        {
            return await _context.TipoNotificaciones.ToListAsync();
        }

        public async Task<TipoNotificacionesEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.TipoNotificaciones
                .FirstOrDefaultAsync(t => t.ClaveTipoNotificacion == clave);
        }

        public async Task<TipoNotificacionesEntity?> ObtenerPorNombreProcesoAsync(string nombreProceso)
        {
            return await _context.TipoNotificaciones
                .FirstOrDefaultAsync(t => t.NombreProceso == nombreProceso);
        }

        public async Task RegistrarAsync(TipoNotificacionRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_insertar_tipo_notificacion(
                    @p_nombre_proceso,
                    @p_descripcion,
                    @p_icono,
                    @p_color
                );",
                new NpgsqlParameter<string>("p_nombre_proceso", dto.NombreProceso),
                new NpgsqlParameter<string>("p_descripcion", dto.Descripcion),
                new NpgsqlParameter<string>("p_icono", dto.Icono),
                new NpgsqlParameter<string>("p_color", dto.Color)
            );
        }
    }
}
