using Npgsql;
using Telesecundaria.DTOs.DetalleRevision;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Telesecundaria.Repositories.Implementations
{
    public class DetalleRevisionRepository : IDetalleRevisionRepository
    {
        private readonly ApplicationDbContext _context;

        public DetalleRevisionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DetalleRevisionEntity>> ListarPorRevisionAsync(string claveRevision)
        {
            return await _context.DetalleRevision
                .Where(d => d.ClaveRevision == claveRevision)
                .ToListAsync();
        }

        public async Task<DetalleRevisionEntity?> ObtenerPorIdAsync(string claveRevision, string claveDocAspirante)
        {
            return await _context.DetalleRevision
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.ClaveRevision == claveRevision && d.ClaveDocAspirante == claveDocAspirante);
        }

        public async Task RegistrarDetalleAsync(DetalleRevisionRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL registrar_detalle_revision(
                    @p_claveRevision,
                    @p_claveDocAspirante,
                    @p_estatus_doc,
                    @p_motivo_rechazo
                );",
                new NpgsqlParameter<string>("p_claveRevision", dto.ClaveRevision),
                new NpgsqlParameter<string>("p_claveDocAspirante", dto.ClaveDocAspirante),
                new NpgsqlParameter<string>("p_estatus_doc", dto.EstatusDoc),
                new NpgsqlParameter<string>("p_motivo_rechazo", dto.MotivoRechazo ?? string.Empty)
            );
        }

        public async Task ActualizarDetalleAsync(string claveRevision, string claveDocAspirante, DetalleRevisionUpdateDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL actualizar_detalle_revision(
                    @p_claveRevision,
                    @p_claveDocAspirante,
                    @p_estatus_doc,
                    @p_motivo_rechazo
                );",
                new NpgsqlParameter<string>("p_claveRevision", claveRevision),
                new NpgsqlParameter<string>("p_claveDocAspirante", claveDocAspirante),
                new NpgsqlParameter<string>("p_estatus_doc", dto.EstatusDoc),
                new NpgsqlParameter<string>("p_motivo_rechazo", dto.MotivoRechazo ?? string.Empty)
            );
        }
    }
}
