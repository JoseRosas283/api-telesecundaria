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

        public async Task<(bool exito, string mensaje, string claveToken, string tokenOriginal, string nombreTutor)>
            IniciarSesionAsync(LoginTutorRequest request, string ip, string userAgent)
        {
            var conn = (NpgsqlConnection)_context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

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

            await using var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();

            bool exito = reader.GetBoolean(0);
            string mensaje = reader.GetString(1);
            string claveToken = exito ? reader.GetString(2) : string.Empty;
            string tokenOriginal = exito ? reader.GetString(3) : string.Empty;
            string nombreTutor = exito ? reader.GetString(4) : string.Empty;

            return (exito, mensaje, claveToken, tokenOriginal, nombreTutor);
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
