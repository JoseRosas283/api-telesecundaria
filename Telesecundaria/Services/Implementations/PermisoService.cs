using Telesecundaria.DTOs.PermisosGestion.Request;
using Telesecundaria.DTOs.PermisosGestion.Response;
using Telesecundaria.Repositories.Implementations;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class PermisoService : IPermisoService
    {
        private readonly IPermisoRepository _repository;

        public PermisoService(IPermisoRepository repository)
        {
            _repository = repository;
        }

        public async Task<PermisoGestionarResponse> GestionarPermisosAsync(PermisoGestionarRequest request)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(request.NombreRol))
                throw new ArgumentException("El nombre del rol es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.NombreModulo))
                throw new ArgumentException("El nombre del módulo es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.PuedeVer))
                throw new ArgumentException("El permiso 'PuedeVer' es obligatorio.");

            // Validar que los valores sean "Puede" o "No puede"
            var valoresPermitidos = new[] { "Puede", "No puede" };
            if (!valoresPermitidos.Contains(request.PuedeVer))
                throw new ArgumentException("PuedeVer debe ser 'Puede' o 'No puede'.");

            // Para los permisos de acciones (si se proporcionan), deben ser "Puede" o "No puede"
            if (request.PuedeCrear != null && !valoresPermitidos.Contains(request.PuedeCrear))
                throw new ArgumentException("PuedeCrear debe ser 'Puede', 'No puede' o null.");
            if (request.PuedeEditar != null && !valoresPermitidos.Contains(request.PuedeEditar))
                throw new ArgumentException("PuedeEditar debe ser 'Puede', 'No puede' o null.");
            if (request.PuedeEliminar != null && !valoresPermitidos.Contains(request.PuedeEliminar))
                throw new ArgumentException("PuedeEliminar debe ser 'Puede', 'No puede' o null.");

            return await _repository.GestionarPermisosAsync(request);
        }
   }
}
