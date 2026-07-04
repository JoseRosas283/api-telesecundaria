using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.TipoDocumentos;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class TipoDocumentosRepository : ITipoDocumentosRepository
    {
        private readonly ApplicationDbContext _context;

        public TipoDocumentosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoDocumentosEntity>> ListarTipoDocumentosAsync()
        {
            return await _context.TipoDocumentos.ToListAsync();
        }

        public async Task<TipoDocumentosEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.TipoDocumentos
                .FirstOrDefaultAsync(t => t.ClaveTipoDocumento == clave);
        }

        public async Task InsertarTipoDocumentoAsync(TipoDocumentoRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_insertar_tipo_documento(
                    @p_nombre_documento,
                    @p_area,
                    @p_descripcion
                );",
                new NpgsqlParameter<string>("p_nombre_documento", dto.NombreDocumento),
                new NpgsqlParameter<string>("p_area", dto.Area),
                new NpgsqlParameter<string>("p_descripcion", dto.Descripcion)
            );
        }
    }
}
