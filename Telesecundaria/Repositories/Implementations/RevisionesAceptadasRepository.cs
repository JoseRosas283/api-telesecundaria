using Npgsql;
using Telesecundaria.DTOs.RevisionesAceptadas;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Telesecundaria.Repositories.Implementations
{
    public class RevisionesAceptadasRepository : IRevisionesAceptadasRepository
    {
        private readonly ApplicationDbContext _context;

        public RevisionesAceptadasRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RevisionesAceptadasEntity>> ListarPendientesAsync()
        {
            return await _context.RevisionesAceptadas
                .Where(r => r.Estado == true)
                .ToListAsync();
        }

        public async Task<RevisionesAceptadasEntity?> ObtenerPorIdAsync(string claveRevision)
        {
            return await _context.RevisionesAceptadas
                .FirstOrDefaultAsync(r => r.ClaveRevision == claveRevision);
        }

        public async Task RegistrarAceptacionAsync(RevisionAceptadaRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL registrar_aceptacion_buffer(
                    @p_claveRevision
                );",
                new NpgsqlParameter<string>("p_claveRevision", dto.ClaveRevision)
            );
        }
    }
}
