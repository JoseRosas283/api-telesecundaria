using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using Telesecundaria.DTOs.Auth.Internal;
using Telesecundaria.DTOs.AuthTutor.Internal;
using Telesecundaria.DTOs.AuthTutor.Request;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class AuthTutorRepository : IAuthTutorRepository
    {
        private readonly ApplicationDbContext _context;
        public AuthTutorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<(bool exito, string mensaje, string claveToken, string tokenOriginal, string nombreTutor, string claveTutorAspirante)>
            IniciarSesionAsync(LoginTutorRequest request, string ip, string userAgent)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            // Llamada original al SP, SIN modificar (9 argumentos)
            await using var cmd = new NpgsqlCommand(
                @"CALL sp_login_tutor_aspirante(
                @p_correo,
                @p_contrasena,
                @p_ip_origen,
                @p_dispositivo_origen,
                NULL, NULL, NULL, NULL, NULL)", conn);
            cmd.Parameters.AddWithValue("p_correo", request.Correo);
            cmd.Parameters.AddWithValue("p_contrasena", request.Contrasenia);
            cmd.Parameters.AddWithValue("p_ip_origen", ip);
            cmd.Parameters.AddWithValue("p_dispositivo_origen", userAgent);

            bool exito;
            string mensaje, claveToken, tokenOriginal, nombreTutor;

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                await reader.ReadAsync();
                exito = reader.GetBoolean(0);
                mensaje = reader.GetString(1);
                claveToken = exito ? reader.GetString(2) : string.Empty;
                tokenOriginal = exito ? reader.GetString(3) : string.Empty;
                nombreTutor = exito ? reader.GetString(4) : string.Empty;
            }

            string claveTutorAspirante = string.Empty;

            // Si el login fue exitoso, buscamos la clave por correo (sin tocar el SP)
            if (exito)
            {
                await using var cmdClave = new NpgsqlCommand(
                    @"SELECT ""claveTutorAspirante"" FROM ""TutorAspirante"" WHERE correo = LOWER(TRIM(@p_correo))",
                    conn);
                cmdClave.Parameters.AddWithValue("p_correo", request.Correo);

                var resultado = await cmdClave.ExecuteScalarAsync();
                claveTutorAspirante = resultado?.ToString() ?? string.Empty;
            }

            return (exito, mensaje, claveToken, tokenOriginal, nombreTutor, claveTutorAspirante);
        }

        public async Task<(bool exito, string mensaje)>
            CerrarSesionAsync(string claveToken, string tokenOriginal)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(
                @"CALL cierre_tutorAspirante_sesion(
                @p_clave_token,
                @p_token_original,
                NULL, NULL)", conn);
            cmd.Parameters.AddWithValue("p_clave_token", claveToken);
            cmd.Parameters.AddWithValue("p_token_original", tokenOriginal);
            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            bool exito = reader.GetBoolean(0);
            string mensaje = reader.GetString(1);
            return (exito, mensaje);
        }


        public async Task<TutorCredencialesDto?> ObtenerCredencialesAsync(string correo)
        {
            return await _context.TutorAspirante
                .Where(u => u.Correo == correo)
                .Select(u => new TutorCredencialesDto
                {
                    ClaveTutorAspirante = u.ClaveTutorAspirante,
                    PasswordHash = u.Contrasena,
                    Estado = u.Estado
                })
                .FirstOrDefaultAsync();
        }

        public async Task GuardarRefreshTokenAsync(string claveTutorAspirante, string claveToken, string refreshToken, DateTime expiracion)
        {

            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();

            if(conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                @"INSERT INTO refresh_tokens_tutor(""claveTutorAspirante"", ""claveTokenConvocatoria"", token, fecha_expiracion)
                VALUES (@p_clave_tutor_aspirante, @p_clave_token_convocatoria, @p_token, @p_expiracion)", conn);

            cmd.Parameters.AddWithValue("p_clave_tutor_aspirante", claveTutorAspirante);
            cmd.Parameters.AddWithValue("p_clave_token_convocatoria", claveToken);
            cmd.Parameters.AddWithValue("p_token", refreshToken);
            cmd.Parameters.AddWithValue("p_expiracion", expiracion);

            await cmd.ExecuteNonQueryAsync();

        }


        public async Task<RefreshTokenTutor?> ValidarRefreshTokenAsync(string refreshToken)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();


            await using var cmd = new NpgsqlCommand(
                @"SELECT ""claveTokenConvocatoria"", ""claveTutorAspirante"" 
                FROM refresh_tokens_tutor
                WHERE token = @p_token
                AND revocado = FALSE
                AND fecha_expiracion > CURRENT_TIMESTAMP", conn);

            cmd.Parameters.AddWithValue("p_token", refreshToken);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new RefreshTokenTutor
                {
                    ClaveToken = reader.GetString(0),
                    ClaveTutorAspirante = reader.GetString(1)
                };
            }

            return null;

        }

        public async Task RevocarRefreshTokenAsync(string refreshToken)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "UPDATE refresh_tokens_tutor SET revocado = TRUE WHERE token = @p_token", conn);

            cmd.Parameters.AddWithValue("p_token", refreshToken);
            await cmd.ExecuteNonQueryAsync();

        }

       public async Task<(bool exito, string mensaje, string claveTutorAspirante, string codigo)>GenerarCodigoRecuperacionAsync(string correo)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                @"CALL sp_generar_codigo_recuperacion_tutor(
        @p_correo,
        NULL, NULL, NULL, NULL)", conn);

            cmd.Parameters.AddWithValue("p_correo", correo);

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            bool exito = reader.GetBoolean(0);
            string mensaje = reader.GetString(1);
            string claveTutorAspirante = exito && !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
            string codigo = exito && !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;

            return (exito, mensaje, claveTutorAspirante, codigo);
        }


        public async Task<(bool exito, string mensaje, string tokenConfirmacion)>ValidarCodigoRecuperacionAsync(string correo, string codigo)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                @"CALL sp_validar_codigo_recuperacion_tutor(
                @p_correo,
                @p_codigo,
                NULL, NULL, NULL)", conn);

            cmd.Parameters.AddWithValue("p_correo", correo);
            cmd.Parameters.AddWithValue("p_codigo", codigo);

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            bool exito = reader.GetBoolean(0);
            string mensaje = reader.GetString(1);
            string tokenConfirmacion = exito && !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;

            return (exito, mensaje, tokenConfirmacion);
        }





        public async Task<(bool exito, string mensaje)>ConfirmarCambioContrasenaAsync(string correo, string tokenConfirmacion, string nuevaContrasenaHash)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                @"CALL sp_confirmar_cambio_contrasena_tutor(
                @p_correo,
                @p_token_confirmacion,
                @p_nueva_contrasena,
                NULL, NULL)", conn);

            cmd.Parameters.AddWithValue("p_correo", correo);
            cmd.Parameters.AddWithValue("p_token_confirmacion", tokenConfirmacion);
            cmd.Parameters.AddWithValue("p_nueva_contrasena", nuevaContrasenaHash);

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            bool exito = reader.GetBoolean(0);
            string mensaje = reader.GetString(1);

            return (exito, mensaje);
        }

    }
}
