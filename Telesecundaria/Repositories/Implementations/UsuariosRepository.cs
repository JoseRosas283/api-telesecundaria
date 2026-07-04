using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Usuarios.Request;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class UsuariosRepository : IUsuariosRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuariosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsuariosEntity>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.Empleado)
                .ThenInclude(e => e.Expediente)
                .Include(u => u.Empleado)
                .ThenInclude(e => e.EmpleadoRoles)
                .ThenInclude(er => er.Rol)
                .Include(u => u.Receptores)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<UsuariosEntity?> GetByIdAsync(string claveUsuario)
        {
            return await _context.Usuarios
                .Include(u => u.Empleado)
                .ThenInclude(e => e.Expediente)
                .Include(u => u.Empleado)
                .ThenInclude(e => e.EmpleadoRoles)
                .ThenInclude(er => er.Rol)
                .Include(u => u.Receptores)
                .FirstOrDefaultAsync(u => u.ClaveUsuario == claveUsuario);
        }


        public async Task<UsuariosEntity> CreateAsync(UsuarioCreateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parameters = new[]
                {
                    new NpgsqlParameter("p_nombre_usuario", NpgsqlTypes.NpgsqlDbType.Varchar)
                    { Value = request.NombreUsuario },

                    new NpgsqlParameter("p_contrasenia", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.Contrasenia },

                    new NpgsqlParameter("p_correo_institucional", NpgsqlTypes.NpgsqlDbType.Varchar)

                        { Value = (object?)request.CorreoInstitucional ?? DBNull.Value },

                    new NpgsqlParameter("p_claveEmpleado", NpgsqlTypes.NpgsqlDbType.Varchar)
                        { Value = request.ClaveEmpleado }
                };

                await _context.Database.ExecuteSqlRawAsync(

                    @"CALL sp_insertar_usuario(  
                       @p_nombre_usuario,
                       @p_contrasenia,
                       @p_correo_institucional,
                       @p_claveEmpleado

                    )",
                    parameters
                    );

                await transaction.CommitAsync();

                var usuarioCreado = await _context.Usuarios
                    .Include(u => u.Empleado)
                    .ThenInclude(e => e.Expediente)
                    .Include(u => u.Empleado)
                    .ThenInclude(e => e.EmpleadoRoles)
                    .ThenInclude(er => er.Rol)
                    .Include(u => u.Receptores)
                    .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario);

                return usuarioCreado;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                var inner = ex.InnerException?.Message ?? "sin inner exception";
                throw new Exception($"Error al crear usuario: {ex.Message} | Inner: {inner}", ex);

            }

        }
        public async Task UpdateAsync(string claveUsuario, UsuarioUpdateRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parameters = new[]
                {
            new NpgsqlParameter("p_claveUsuario", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = claveUsuario },

            new NpgsqlParameter("p_nuevo_nombre", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = request.NombreUsuario },

            new NpgsqlParameter("p_nueva_contrasenia", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = request.Contrasenia ?? string.Empty },

            new NpgsqlParameter("p_nuevo_correo", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = (object?)request.CorreoInstitucional ?? DBNull.Value }
        };

                await _context.Database.ExecuteSqlRawAsync(
                @"CALL sp_actualizar_usuario(
                @p_claveUsuario,
                @p_nuevo_nombre,
                @p_nueva_contrasenia,
                @p_nuevo_correo
                )",
                 parameters
                );

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var inner = ex.InnerException?.Message ?? "sin inner exception";
                throw new Exception($"Error al actualizar usuario: {ex.Message} | Inner: {inner}", ex);
            }

        }

        public async Task DeleteAsync(string claveUsuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var parameters = new[]
                {
            new NpgsqlParameter("p_claveUsuario", NpgsqlTypes.NpgsqlDbType.Varchar)
                { Value = claveUsuario }
        };

                await _context.Database.ExecuteSqlRawAsync(
                    @"CALL sp_eliminar_usuario(@p_claveUsuario)",
                    parameters
                );

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var inner = ex.InnerException?.Message ?? "sin inner exception";
                throw new Exception($"Error al eliminar usuario: {ex.Message} | Inner: {inner}", ex);

            }
        }
    }
}
