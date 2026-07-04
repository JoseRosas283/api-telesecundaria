using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.EmpleadoRol.Request;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class EmpleadoRolRepository : IEmpleadoRolRepository
    {
        private readonly ApplicationDbContext _context;

        public EmpleadoRolRepository(ApplicationDbContext context)
        {

            _context = context;

        }

        public async Task<IEnumerable<EmpleadoRolEntity>> GetAllAsync()
        {
            return await _context.EmpleadoRol
                .Include(e => e.Empleado)
                .Include(e => e.Rol)
                .ToListAsync();
        }

        public async Task<EmpleadoRolEntity?> GetByIdAsync(string claveRol)
        {
            return await _context.EmpleadoRol
                .Include(e => e.Empleado)
                .Include(e => e.Rol)
                .FirstOrDefaultAsync(e => e.ClaveRol == claveRol);
        }

        public async Task<EmpleadoRolEntity> CreateAsync(EmpleadoRolCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parameters = new[]
                {
                    new NpgsqlParameter("p_claveEmpleado", NpgsqlTypes.NpgsqlDbType.Varchar)
                    { Value = request.ClaveEmpleado },

                    new NpgsqlParameter("p_NombreRol", NpgsqlTypes.NpgsqlDbType.Varchar)
                    { Value = request.NombreRol }

                };

                await _context.Database.ExecuteSqlRawAsync(
                   @"CALL sp_asignar_rol_empleado(
                    @p_claveEmpleado,
                    @p_nombreRol
                    )",
                   parameters

                    );

                await transaction.CommitAsync();

                var EmpleadoRolCreado = await _context.EmpleadoRol
                    .Include(e => e.Empleado)
                    .Include(e => e.Rol)
                    .FirstOrDefaultAsync(e => e.ClaveEmpleado == request.ClaveEmpleado);

                return EmpleadoRolCreado;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al asignar rol: {ex.Message}", ex);

            }
        }
    }
}
