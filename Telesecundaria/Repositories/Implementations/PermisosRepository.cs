using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System.Data;
using Telesecundaria.DTOs.PermisosGestion.Request;
using Telesecundaria.DTOs.PermisosGestion.Response;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class PermisosRepository : IPermisoRepository
    {
        private readonly ApplicationDbContext _context;

        public PermisosRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PermisoGestionarResponse> GestionarPermisosAsync(PermisoGestionarRequest request)
        {
            using (var connection = (NpgsqlConnection)_context.Database.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var cmd = new NpgsqlCommand("sp_gestionar_permisos_por_nombre", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros de entrada
                    cmd.Parameters.AddWithValue("p_nombre_rol", request.NombreRol ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_nombre_modulo", request.NombreModulo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_puede_ver", request.PuedeVer ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_puede_crear", request.PuedeCrear ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_puede_editar", request.PuedeEditar ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("p_puede_eliminar", request.PuedeEliminar ?? (object)DBNull.Value);

                    // Parámetros de salida
                    var pExito = new NpgsqlParameter("p_exito", NpgsqlTypes.NpgsqlDbType.Boolean)
                    {
                        Direction = ParameterDirection.Output
                    };
                    var pMensaje = new NpgsqlParameter("p_mensaje", NpgsqlTypes.NpgsqlDbType.Varchar)
                    {
                        Direction = ParameterDirection.Output,
                        Size = 255
                    };
                    cmd.Parameters.Add(pExito);
                    cmd.Parameters.Add(pMensaje);

                    // Ejecutar
                    await cmd.ExecuteNonQueryAsync();

                    // Leer salidas
                    bool exito = pExito.Value != DBNull.Value && (bool)pExito.Value;
                    string mensaje = pMensaje.Value != DBNull.Value ? (string)pMensaje.Value : string.Empty;

                    return new PermisoGestionarResponse
                    {
                        Exito = exito,
                        Mensaje = mensaje
                    };
                }
            }
        }
    }
}

