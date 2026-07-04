using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Roles.Request;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class RolesRepository : IRolesRepository
    {
        private readonly ApplicationDbContext _context;

        public RolesRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<RolesEntity>> GetAllAsync()
        {
            return await _context.Roles
                .Include(e => e.Permisos)
                .Include(e => e.EmpleadoRoles)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<RolesEntity?> GetByIdAsync(string claveRol)
        {
            return await (_context.Roles
                .Include(e => e.Permisos)
                .Include(e => e.EmpleadoRoles)
                .FirstOrDefaultAsync(e => e.ClaveRol == claveRol));
        }

        public async Task<RolesEntity> CreateAsync(RolesCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var parameters = new[]
                {
            new NpgsqlParameter("p_nombre_rol", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = request.NombreRol },
            new NpgsqlParameter("p_descripcion", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = request.Descripcion }
        };

                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL sp_insertar_rol(
            @p_nombre_rol,
            @p_descripcion
            )",
                    parameters
                );

                await transaction.CommitAsync();

                var rolCreado = await _context.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.NombreRol == request.NombreRol.Trim());

                return rolCreado!;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al crear rol: {ex.Message}", ex);
            }
        }
    }
}
