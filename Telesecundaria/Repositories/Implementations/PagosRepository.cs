using Npgsql;
using Telesecundaria.DTOs.Pagos;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Telesecundaria.Repositories.Implementations
{
    public class PagosRepository : IPagosRepository
    {
        private readonly ApplicationDbContext _context;

        public PagosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PagosEntity>> ListarPagosAsync()
        {
            return await _context.Pagos.ToListAsync();
        }

        public async Task<PagosEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Pagos
                .FirstOrDefaultAsync(p => p.ClavePago == clave);
        }

        public async Task RegistrarPagoAsync(PagoRequestDTO dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_registrar_pago(
                    @p_claveTutor,
                    @p_nombre_usuario,
                    @p_monto,
                    @p_metodo_pago,
                    @p_comprobante_pago
                );",
                new NpgsqlParameter<string>("p_claveTutor", dto.ClaveTutor),
                new NpgsqlParameter<string>("p_nombre_usuario", dto.NombreUsuario),
                new NpgsqlParameter<decimal>("p_monto", dto.Monto),
                new NpgsqlParameter<string>("p_metodo_pago", dto.MetodoPago),
                new NpgsqlParameter<string>("p_comprobante_pago", dto.ComprobantePago ?? string.Empty)
            );
        }
    }
}
