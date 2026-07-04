using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class AdjuncionesOriginalesRepository : IAdjuncionesOriginalesRepository
    {
        private readonly ApplicationDbContext _context;

        public AdjuncionesOriginalesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CrearAdjuncionOriginalAsync(string claveEntrega, string claveUsuario)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using (var setSchema = new NpgsqlCommand("SET search_path TO public", conn))
                await setSchema.ExecuteNonQueryAsync();

            using (var cmd = new NpgsqlCommand(
                "CALL sp_registrar_adjuncion_original(@entrega::varchar, @usuario::varchar)", conn))
            {
                cmd.Parameters.AddWithValue("entrega", claveEntrega);
                cmd.Parameters.AddWithValue("usuario", claveUsuario);
                await cmd.ExecuteNonQueryAsync();
            }

            using var query = new NpgsqlCommand(
                @"SELECT ""claveAdjOriginal"" FROM ""AdjuncionesOriginales"" 
                  WHERE ""claveEntrega"" = @entrega 
                  ORDER BY ""claveAdjOriginal"" DESC 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("entrega", claveEntrega);
            var result = await query.ExecuteScalarAsync();
            return result?.ToString() ?? string.Empty;
        }

        public async Task<AdjuncionesOriginalesEntity?> ObtenerAdjuncionOriginalAsync(string claveAdjOriginal)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT ""claveAdjOriginal"", ""claveEntrega"", ""claveUsuario"", fecha_carga
                  FROM ""AdjuncionesOriginales"" 
                  WHERE ""claveAdjOriginal"" = @clave 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("clave", claveAdjOriginal);

            using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new AdjuncionesOriginalesEntity
                {
                    ClaveAdjOriginal = reader.GetString(0),
                    ClaveEntrega = reader.GetString(1),
                    ClaveUsuario = reader.GetString(2),
                    FechaCarga = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
                };
            }
            return null;
        }

        public async Task<AdjuncionesOriginalesEntity?> ObtenerPorEntregaAsync(string claveEntrega)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT ""claveAdjOriginal"", ""claveEntrega"", ""claveUsuario"", fecha_carga
                  FROM ""AdjuncionesOriginales"" 
                  WHERE ""claveEntrega"" = @entrega 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("entrega", claveEntrega);

            using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new AdjuncionesOriginalesEntity
                {
                    ClaveAdjOriginal = reader.GetString(0),
                    ClaveEntrega = reader.GetString(1),
                    ClaveUsuario = reader.GetString(2),
                    FechaCarga = reader.IsDBNull(3) ? null : reader.GetDateTime(3)
                };
            }
            return null;
        }

        public async Task InsertarDetalleAdjuncionOriginalAsync(string claveAdjOriginal, string claveDocAspirante)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using (var setSchema = new NpgsqlCommand("SET search_path TO public", conn))
                await setSchema.ExecuteNonQueryAsync();

            using var cmd = new NpgsqlCommand(
                "CALL sp_registrar_detalle_adjuncion_original(@adj::varchar, @doc::varchar)", conn);

            cmd.Parameters.AddWithValue("adj", claveAdjOriginal);
            cmd.Parameters.AddWithValue("doc", claveDocAspirante);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<DetalleAdjuncionOriginalEntity>> ObtenerDetallesAsync(string claveAdjOriginal)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT ""claveAdjOriginal"", ""claveDocAspirante"", ruta_pdf_original
                  FROM ""DetalleAdjuncionOriginal"" 
                  WHERE ""claveAdjOriginal"" = @clave", conn);

            query.Parameters.AddWithValue("clave", claveAdjOriginal);

            var lista = new List<DetalleAdjuncionOriginalEntity>();
            using var reader = await query.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DetalleAdjuncionOriginalEntity
                {
                    ClaveAdjOriginal = reader.GetString(0),
                    ClaveDocAspirante = reader.GetString(1),
                    RutaPdfOriginal = reader.GetString(2)
                });
            }
            return lista;
        }

        public async Task<int> ContarRequisitosInscripcionAsync()
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT COUNT(*) FROM ""Requisitos"" 
                  WHERE etapa_proceso = 'Inscripción' 
                    AND estado_requisito = TRUE", conn);

            var result = await query.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<string?> ObtenerEstadoEntregaAsync(string claveEntrega)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT estado_final FROM ""Entregas"" 
                  WHERE ""claveEntrega"" = @entrega 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("entrega", claveEntrega);
            var result = await query.ExecuteScalarAsync();
            return result?.ToString();
        }
    }
}
