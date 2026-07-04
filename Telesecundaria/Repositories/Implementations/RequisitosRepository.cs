using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Requisitos;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class RequisitosRepository : IRequisitosRepository
    {
        private readonly ApplicationDbContext _context;

        public RequisitosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RequisitosEntity>> ListarRequisitosAsync()
        {
            return await _context.Requisitos.ToListAsync();
        }

        public async Task<RequisitosEntity?> ObtenerPorIdAsync(string claveRequisito)
        {
            return await _context.Requisitos
                .FirstOrDefaultAsync(r => r.ClaveRequisito == claveRequisito);
        }

        public async Task ConfigurarRequisitoEtapaAsync(RequisitoRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_configurar_requisito_etapa(
                    @p_etapa_proceso,
                    @p_nombre_documento
                );",
                new NpgsqlParameter<string>("p_etapa_proceso", dto.EtapaProceso),
                new NpgsqlParameter<string>("p_nombre_documento", dto.NombreDocumento)
            );
        }
    }
}
