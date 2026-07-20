using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class EntregasRepository : IEntregasRepository
    {
        private readonly ApplicationDbContext _context;

        public EntregasRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CrearEntregaAsync(string claveCita, string claveUsuario)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using (var setSchema = new NpgsqlCommand("SET search_path TO public", conn))
                await setSchema.ExecuteNonQueryAsync();

            using (var cmd = new NpgsqlCommand(
                "CALL sp_inicializar_entrega(@cita::varchar, @usuario::varchar)", conn))
            {
                cmd.Parameters.AddWithValue("cita", claveCita);
                cmd.Parameters.AddWithValue("usuario", claveUsuario);
                await cmd.ExecuteNonQueryAsync();
            }

            using var query = new NpgsqlCommand(
                @"SELECT ""claveEntrega"" FROM ""Entregas"" 
                  WHERE ""claveCita"" = @cita 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("cita", claveCita);
            var result = await query.ExecuteScalarAsync();
            return result?.ToString() ?? string.Empty;
        }

        public async Task<EntregasEntity?> ObtenerEntregaAsync(string claveEntrega)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT ""claveEntrega"", fecha_formalizacion, estado_final, 
                         ""claveCita"", ""claveAspirante"", ""claveTutorAspirante"", ""claveUsuario""
                  FROM ""Entregas"" 
                  WHERE ""claveEntrega"" = @clave 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("clave", claveEntrega);

            using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new EntregasEntity
                {
                    ClaveEntrega = reader.GetString(0),
                    FechaFormalizacion = reader.GetDateTime(1),
                    EstadoFinal = reader.GetString(2),
                    ClaveCita = reader.GetString(3),
                    ClaveAspirante = reader.GetString(4),
                    ClaveTutorAspirante = reader.GetString(5),
                    ClaveUsuario = reader.GetString(6)
                };
            }
            return null;
        }

        public async Task<EntregasEntity?> ObtenerPorCitaAsync(string claveCita)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT ""claveEntrega"", fecha_formalizacion, estado_final, 
                         ""claveCita"", ""claveAspirante"", ""claveTutorAspirante"", ""claveUsuario""
                  FROM ""Entregas"" 
                  WHERE ""claveCita"" = @cita 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("cita", claveCita);

            using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new EntregasEntity
                {
                    ClaveEntrega = reader.GetString(0),
                    FechaFormalizacion = reader.GetDateTime(1),
                    EstadoFinal = reader.GetString(2),
                    ClaveCita = reader.GetString(3),
                    ClaveAspirante = reader.GetString(4),
                    ClaveTutorAspirante = reader.GetString(5),
                    ClaveUsuario = reader.GetString(6)
                };
            }
            return null;
        }
    }
}
