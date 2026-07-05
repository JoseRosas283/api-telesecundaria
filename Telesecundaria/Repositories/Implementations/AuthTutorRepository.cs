using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
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
    }
}
