using System.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Telesecundaria.DTOs.Grupos;
using Telesecundaria.Models;
using Telesecundaria.Persistence;
using Telesecundaria.Repositories.Interfaces;

namespace Telesecundaria.Repositories.Implementations
{
    public class GruposRepository : IGruposRepository
    {
        private readonly ApplicationDbContext _context;

        public GruposRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GruposEntity>> ListarGruposAsync()
        {
            return await _context.Grupos.ToListAsync();
        }

        public async Task<GruposEntity?> ObtenerPorIdAsync(string clave)
        {
            return await _context.Grupos
                .FirstOrDefaultAsync(g => g.ClaveGrupo == clave);
        }

        public async Task<GruposEntity> RegistrarGrupoAsync(GrupoRequestDTO dto)
        {
            var grupo = new GruposEntity
            {
                Grado = dto.Grado,
                Seccion = dto.Seccion,
                CapacidadMaxima = dto.CapacidadMaxima,
                Generacion = dto.Generacion,
                Estado = true
            };

            _context.Grupos.Add(grupo);
            await _context.SaveChangesAsync();
            return grupo;
        }
    }
}
