using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class AdjuncionesRepository : IAdjuncionesRepository
    {
        private readonly ApplicationDbContext _context;

        public AdjuncionesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CrearAdjuncionAsync(string claveTutor, string claveAspirante)
        {
            // Abre y obtiene la conexión de EF el DbContext
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            // Define que se trabajara sobre el esquema public
            using (var setSchema = new NpgsqlCommand("SET search_path TO public", conn))
                await setSchema.ExecuteNonQueryAsync();

            // Ejecuta el SP 
            using (var cmd = new NpgsqlCommand(
                "CALL sp_crear_adjuncion_segura(@tutor::varchar, @aspirante::varchar, NULL::varchar)", conn))
            {
                cmd.Parameters.AddWithValue("tutor", claveTutor);
                cmd.Parameters.AddWithValue("aspirante", claveAspirante);
                await cmd.ExecuteNonQueryAsync();
            }

            // Consulta la clave generada recientemente y la devuelve al servicio
            using var query = new NpgsqlCommand(
                @"SELECT ""claveAdjuncion"" FROM ""Adjunciones"" 
                  WHERE ""claveAspirante"" = @aspirante 
                    AND estatus_operativo = 'Abierta' 
                  ORDER BY ""claveAdjuncion"" DESC 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("aspirante", claveAspirante);
            var result = await query.ExecuteScalarAsync();
            return result?.ToString() ?? string.Empty;
        }

        public async Task<string> InsertarDocumentoAspirante(string rutaArchivo, string claveAspirante, string nombreTipoDocumento)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using (var setSchema = new NpgsqlCommand("SET search_path TO public", conn))
                await setSchema.ExecuteNonQueryAsync();

            using (var cmd = new NpgsqlCommand(
                "CALL sp_insertar_documento_aspirante(@ruta::varchar, @aspirante::varchar, @tipo::varchar, NULL::varchar)", conn))
            {
                cmd.Parameters.AddWithValue("ruta", rutaArchivo);
                cmd.Parameters.AddWithValue("aspirante", claveAspirante);
                cmd.Parameters.AddWithValue("tipo", nombreTipoDocumento);
                await cmd.ExecuteNonQueryAsync();
            }

            using var query = new NpgsqlCommand(
                @"SELECT ""claveDocAspirante"" FROM ""DocumentosAspirante"" 
                  WHERE ruta_archivo = @ruta 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("ruta", rutaArchivo);
            var result = await query.ExecuteScalarAsync();
            return result?.ToString() ?? string.Empty;
        }

        public async Task InsertarDetalleAdjuncionAsync(string claveAdjuncion, string claveDocAspirante)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using (var setSchema = new NpgsqlCommand("SET search_path TO public", conn))
            {
                await setSchema.ExecuteNonQueryAsync();
            }

            using var cmd = new NpgsqlCommand(
                "CALL sp_insertar_detalle_adjuncion(@adj::varchar, @doc::varchar, NULL::text)", conn);

            cmd.Parameters.AddWithValue("adj", claveAdjuncion);
            cmd.Parameters.AddWithValue("doc", claveDocAspirante);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<AdjuncionesEntity?> ObtenerAdjuncionAsync(string claveAdjuncion)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT ""claveAdjuncion"", ""claveTutorAspirante"", ""claveAspirante"", estatus_gral, estatus_operativo
                  FROM ""Adjunciones"" 
                  WHERE ""claveAdjuncion"" = @clave 
                  LIMIT 1", conn);

            query.Parameters.AddWithValue("clave", claveAdjuncion);

            using var reader = await query.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new AdjuncionesEntity
                {
                    ClaveAdjuncion = reader.GetString(0),
                    ClaveTutorAspirante = reader.GetString(1),
                    ClaveAspirante = reader.GetString(2),
                    EstatusGral = reader.GetString(3),
                    EstatusOperativo = reader.GetString(4)
                };
            }
            return null;
        }

        public async Task<List<DocumentosAspiranteEntity>> ObtenerDocumentosAdjuncionAsync(string claveAdjuncion)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT d.""claveDocAspirante"", d.ruta_archivo, d.""claveTipoDocumento"", d.""claveAspirante""
                  FROM ""DocumentosAspirante"" d
                  JOIN ""DetalleAdjuncion"" da ON da.""claveDocAspirante"" = d.""claveDocAspirante""
                  WHERE da.""claveAdjuncion"" = @clave", conn);

            query.Parameters.AddWithValue("clave", claveAdjuncion);

            var lista = new List<DocumentosAspiranteEntity>();
            using var reader = await query.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DocumentosAspiranteEntity
                {
                    ClaveDocAspirante = reader.GetString(0),
                    RutaArchivo = reader.GetString(1),
                    ClaveTipoDocumento = reader.GetString(2),
                    ClaveAspirante = reader.GetString(3)
                });
            }
            return lista;
        }

        public async Task<List<DocumentosAspiranteEntity>> ObtenerDocumentosPorAspiranteAsync(string claveAspirante)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT d.""claveDocAspirante"", d.ruta_archivo, d.""claveTipoDocumento"", d.""claveAspirante"", td.nombre_documento
                  FROM ""DocumentosAspirante"" d
                  JOIN ""TipoDocumentos"" td ON td.""claveTipoDocumento"" = d.""claveTipoDocumento""
                  WHERE d.""claveAspirante"" = @aspirante", conn);

            query.Parameters.AddWithValue("aspirante", claveAspirante);

            var lista = new List<DocumentosAspiranteEntity>();
            using var reader = await query.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DocumentosAspiranteEntity
                {
                    ClaveDocAspirante = reader.GetString(0),
                    RutaArchivo = reader.GetString(1),
                    ClaveTipoDocumento = reader.GetString(4), // nombre_documento legible
                    ClaveAspirante = reader.GetString(3)
                });
            }
            return lista;
        }

        public async Task<List<DocumentosAspiranteEntity>> ObtenerDocumentosSinDetalleAsync(string claveAspirante)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT d.""claveDocAspirante"", d.ruta_archivo, d.""claveTipoDocumento"", d.""claveAspirante"", td.nombre_documento
                  FROM ""DocumentosAspirante"" d
                  JOIN ""TipoDocumentos"" td ON td.""claveTipoDocumento"" = d.""claveTipoDocumento""
                  WHERE d.""claveAspirante"" = @aspirante
                    AND NOT EXISTS (
                        SELECT 1 FROM ""DetalleAdjuncion"" da 
                        WHERE da.""claveDocAspirante"" = d.""claveDocAspirante""
                    )", conn);

            query.Parameters.AddWithValue("aspirante", claveAspirante);

            var lista = new List<DocumentosAspiranteEntity>();
            using var reader = await query.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DocumentosAspiranteEntity
                {
                    ClaveDocAspirante = reader.GetString(0),
                    RutaArchivo = reader.GetString(1),
                    ClaveTipoDocumento = reader.GetString(4), // nombre_documento legible
                    ClaveAspirante = reader.GetString(3)
                });
            }
            return lista;
        }

        public async Task<List<DocumentoConEstatusProjection>> ObtenerDocumentosConEstatusAsync(string claveAspirante)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var query = new NpgsqlCommand(
                @"SELECT 
                    d.""claveDocAspirante"",
                    d.ruta_archivo,
                    td.nombre_documento,
                    d.""claveAspirante"",
                    da.estatus_documento
                  FROM ""DocumentosAspirante"" d
                  JOIN ""TipoDocumentos"" td ON td.""claveTipoDocumento"" = d.""claveTipoDocumento""
                  LEFT JOIN LATERAL (
                      SELECT estatus_documento
                      FROM ""DetalleAdjuncion""
                      WHERE ""claveDocAspirante"" = d.""claveDocAspirante""
                      ORDER BY fecha_evaluacion DESC, ""claveAdjuncion"" DESC
                      LIMIT 1
                  ) da ON TRUE
                  WHERE d.""claveAspirante"" = @aspirante", conn);

            query.Parameters.AddWithValue("aspirante", claveAspirante);

            var lista = new List<DocumentoConEstatusProjection>();
            using var reader = await query.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DocumentoConEstatusProjection
                {
                    ClaveDocAspirante = reader.GetString(0),
                    RutaArchivo = reader.GetString(1),
                    NombreTipoDocumento = reader.GetString(2),
                    ClaveAspirante = reader.GetString(3),
                    Estatus = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                });
            }
            return lista;
        }

        public async Task ActualizarRutaDocumentoRechazadoAsync(string claveDocAspirante, string nuevaRuta)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using (var setSchema = new NpgsqlCommand("SET search_path TO public", conn))
                await setSchema.ExecuteNonQueryAsync();

            using var cmd = new NpgsqlCommand(
                "CALL sp_actualizar_ruta_documento_rechazado(@doc::varchar, @ruta::varchar)", conn);

            cmd.Parameters.AddWithValue("doc", claveDocAspirante);
            cmd.Parameters.AddWithValue("ruta", nuevaRuta);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<DocumentosAspiranteEntity>> ObtenerDocumentosRechazadosAsync(string claveAspirante)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            // Trae documentos del aspirante cuyo último estatus en DetalleAdjuncion es 'Rechazado'
            using var query = new NpgsqlCommand(
                @"SELECT 
                    d.""claveDocAspirante"",
                    d.ruta_archivo,
                    td.nombre_documento,
                    d.""claveAspirante""
                  FROM ""DocumentosAspirante"" d
                  JOIN ""TipoDocumentos"" td ON td.""claveTipoDocumento"" = d.""claveTipoDocumento""
                  JOIN LATERAL (
                      SELECT estatus_documento
                      FROM ""DetalleAdjuncion""
                      WHERE ""claveDocAspirante"" = d.""claveDocAspirante""
                      ORDER BY fecha_evaluacion DESC, ""claveAdjuncion"" DESC
                      LIMIT 1
                  ) ultimo ON ultimo.estatus_documento = 'Rechazado'
                  WHERE d.""claveAspirante"" = @aspirante", conn);

            query.Parameters.AddWithValue("aspirante", claveAspirante);

            var lista = new List<DocumentosAspiranteEntity>();
            using var reader = await query.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new DocumentosAspiranteEntity
                {
                    ClaveDocAspirante = reader.GetString(0),
                    RutaArchivo = reader.GetString(1),
                    ClaveTipoDocumento = reader.GetString(2), // nombre legible desde TipoDocumentos
                    ClaveAspirante = reader.GetString(3)
                });
            }
            return lista;
        }
    }
}
