using Npgsql;
using Telesecundaria.DTOs.CitasInscripcion;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Telesecundaria.Repositories.Implementations
{
    public class CitasInscripcionRepository : ICitasInscripcionRepository
    {
        private readonly ApplicationDbContext _context;

        public CitasInscripcionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CitasInscripcionEntity>> ListarCitasAsync()
        {
            return await _context.CitasInscripcion.ToListAsync();
        }

        public async Task<CitasInscripcionEntity?> ObtenerPorIdAsync(string claveCita)
        {
            return await _context.CitasInscripcion
                .FirstOrDefaultAsync(c => c.ClaveCita == claveCita);
        }

        public async Task AgendarCitaAsync(CitaInscripcionRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_agendar_cita_individual(
                    @p_clave_revision,
                    @p_fecha_cita,
                    @p_hora_cita
                );",
                new NpgsqlParameter("p_clave_revision", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = dto.ClaveRevision },
                new NpgsqlParameter("p_fecha_cita", NpgsqlTypes.NpgsqlDbType.Date) { Value = dto.FechaCita },
                new NpgsqlParameter("p_hora_cita", NpgsqlTypes.NpgsqlDbType.Time) { Value = dto.HoraCita }
            );
        }

        public async Task<CitasInscripcionEntity?> ObtenerPorRevisionAsync(string claveRevision)
        {
            return await _context.CitasInscripcion
                .FirstOrDefaultAsync(c => c.ClaveRevision == claveRevision);
        }
    }
}
