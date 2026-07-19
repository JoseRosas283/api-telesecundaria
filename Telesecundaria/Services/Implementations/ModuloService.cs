using Telesecundaria.DTOs.Modulo.Request;
using Telesecundaria.DTOs.Modulo.Response;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class ModuloService : IModuloService
    {
        private readonly IModuloRepository _repository;

        public ModuloService(IModuloRepository repository)
        {
            _repository = repository;
        }

        private static ModuloResponse MapToResponse(ModulosEntity m) => new()
        {
            ClaveModulo = m.ClaveModulo,
            NombreModulo = m.NombreModulo,
            Descripcion = m.Descripcion,
            UrlModulo = m.UrlModulo,
            EstadoModulo = m.EstadoModulo,
            ClaveModuloPadre = m.ClaveModuloPadre,
            NombreModuloPadre = m.ModuloPadre?.NombreModulo // solo el nombre, no el objeto completo
        };

        public async Task<IEnumerable<ModuloResponse>> GetAllAsync()
        {
            var modulos = await _repository.GetAllAsync();
            return modulos.Select(MapToResponse);
        }

        public async Task<ModuloResponse?> GetByIdAsync(string claveModulo)
        {
            if (string.IsNullOrWhiteSpace(claveModulo))
                throw new ArgumentException("La clave del módulo no puede estar vacía.");

            var modulo = await _repository.GetByIdAsync(claveModulo);
            return modulo == null ? null : MapToResponse(modulo);
        }

        public async Task<ModuloResponse?> GetByNombreAsync(string nombreModulo)
        {
            if (string.IsNullOrWhiteSpace(nombreModulo))
                throw new ArgumentException("El nombre del módulo no puede estar vacío.");

            var modulo = await _repository.GetByNombreAsync(nombreModulo);
            return modulo == null ? null : MapToResponse(modulo);
        }

        public async Task<ModuloResponse> CreateAsync(ModuloCreateRequest request)
        {
         
            var creado = await _repository.CreateAsync(request);
            return MapToResponse(creado);
        }
    }
}
