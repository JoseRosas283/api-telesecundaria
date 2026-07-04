using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Requisitos;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class RequisitosService : IRequisitosService
    {
        private readonly IRequisitosRepository _repository;

        public RequisitosService(IRequisitosRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<RequisitoResponseDTO>> ListarRequisitosAsync()
        {
            var requisitos = await _repository.ListarRequisitosAsync();
            return requisitos.Select(r => MapearResponse(r));
        }

        public async Task<RequisitoResponseDTO?> ObtenerPorIdAsync(string claveRequisito)
        {
            if (string.IsNullOrWhiteSpace(claveRequisito))
                throw new ArgumentException("La clave del requisito es obligatoria.");

            var requisito = await _repository.ObtenerPorIdAsync(claveRequisito);
            if (requisito == null)
                return null;

            return MapearResponse(requisito);
        }

        public async Task<RequisitoResponseDTO> ConfigurarRequisitoEtapaAsync(RequisitoRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EtapaProceso))
                throw new ArgumentException("La etapa del proceso es obligatoria.");

            if (string.IsNullOrWhiteSpace(dto.NombreDocumento))
                throw new ArgumentException("El nombre del documento es obligatorio.");

            await _repository.ConfigurarRequisitoEtapaAsync(dto);

            var todos = await _repository.ListarRequisitosAsync();
            var insertado = todos
                .LastOrDefault(r => r.EtapaProceso == dto.EtapaProceso.Trim());

            return MapearResponse(insertado!);
        }

        private static RequisitoResponseDTO MapearResponse(RequisitosEntity r) =>
            new RequisitoResponseDTO
            {
                ClaveRequisito = r.ClaveRequisito,
                EtapaProceso = r.EtapaProceso,
                EstadoRequisito = r.EstadoRequisito,
                FormatoExigido = r.FormatoExigido,
                ClaveTipoDocumento = r.ClaveTipoDocumento
            };
    }
}
