using Telesecundaria.DTOs;
using Telesecundaria.DTOs.Adjunciones;
using Telesecundaria.Models;
using Telesecundaria.Repositories.Interfaces;
using Telesecundaria.Services.Interfaces;

namespace Telesecundaria.Services.Implementations
{
    public class AdjuncionesService : IAdjuncionesService
    {
        private readonly IAdjuncionesRepository _repository;
        private readonly IPdfService _pdfService;

        // Catálogo de los 5 tipos requeridos, en un solo lugar para reusar
        private static readonly List<string> TiposRequeridos = new()
        {
            "ACTA DE NACIMIENTO",
            "CURP",
            "COMPROBANTE DE DOMICILIO",
            "CERTIFICADO DE PRIMARIA",
            "CONSTANCIA DE ESTUDIOS VIGENTE"
        };

        public AdjuncionesService(IAdjuncionesRepository repository, IPdfService pdfService)
        {
            _repository = repository;
            _pdfService = pdfService;
        }

        public async Task<AdjuncionResponseDTO> RegistrarAdjuncionAsync(AdjuncionRequestDTO dto)
        {
         
            if (string.IsNullOrWhiteSpace(dto.ClaveTutor))
                throw new ArgumentException("La clave del tutor es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.ClaveAspirante))
                throw new ArgumentException("La clave del aspirante es obligatoria.");

            // Lista de documentos recibidos alineandolo con el catálogo en la DB
            var documentos = new List<(IFormFile Archivo, string NombreTipo)> 
            {
                (dto.ActaNacimiento,       "ACTA DE NACIMIENTO"),
                (dto.Curp,                 "CURP"),
                (dto.ComprobanteDomicilio, "COMPROBANTE DE DOMICILIO"),
                (dto.CertificadoPrimaria,  "CERTIFICADO DE PRIMARIA"),
                (dto.ConstanciaEstudios,   "CONSTANCIA DE ESTUDIOS VIGENTE")
            };

            // Valida que todos los archivos sean PDFs
            foreach (var (archivo, tipo) in documentos)
            {
                if (archivo == null || archivo.Length == 0)
                    throw new ArgumentException($"El documento '{tipo}' es obligatorio.");
                if (Path.GetExtension(archivo.FileName).ToLower() != ".pdf")
                    throw new ArgumentException($"El documento '{tipo}' debe ser un archivo PDF.");
            }

            // PASO 1 Crear la adjunción y obtener su clave
            var claveAdjuncion = await _repository.CrearAdjuncionAsync(dto.ClaveTutor, dto.ClaveAspirante);

            if (string.IsNullOrWhiteSpace(claveAdjuncion))
                throw new Exception("No se pudo generar la clave de adjunción.");

            // PASOS 2 y 3 por cada documento: guarda su PDF, va insertar a doc y luego su detalle
            var documentosRespuesta = new List<DocumentoAdjuntadoDTO>();

            foreach (var (archivo, nombreTipo) in documentos)
            {
                // Convierte el PDF a URL única por tipo
                var rutaUrl = await _pdfService.GuardarPdfAsync(archivo, nombreTipo);

                // Inserta en DocumentosAspirante y obtiene la clave
                var claveDoc = await _repository.InsertarDocumentoAspirante(rutaUrl, dto.ClaveAspirante, nombreTipo);

                // Inserta en DetalleAdjuncion
                await _repository.InsertarDetalleAdjuncionAsync(claveAdjuncion, claveDoc);

                documentosRespuesta.Add(new DocumentoAdjuntadoDTO
                {
                    ClaveDocAspirante = claveDoc,
                    TipoDocumento = nombreTipo,
                    RutaUrl = rutaUrl
                });
            }

            // Obtiene la adjunción completa para la respuesta
            var adjuncion = await _repository.ObtenerAdjuncionAsync(claveAdjuncion);

            return new AdjuncionResponseDTO
            {
                ClaveAdjuncion = claveAdjuncion,
                ClaveTutor = dto.ClaveTutor,
                ClaveAspirante = dto.ClaveAspirante,
                EstatusGral = adjuncion?.EstatusGral ?? "Pendiente",
                Documentos = documentosRespuesta
            };
        }

        public async Task<DocumentoAdjuntadoDTO> RegistrarDocumentoTempAsync(DocumentoTempRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveAspirante))
                throw new ArgumentException("La clave del aspirante es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.TipoDocumento))
                throw new ArgumentException("El tipo de documento es obligatorio.");
            if (dto.Archivo == null || dto.Archivo.Length == 0)
                throw new ArgumentException("El archivo es obligatorio.");
            if (Path.GetExtension(dto.Archivo.FileName).ToLower() != ".pdf")
                throw new ArgumentException("El archivo debe ser un PDF.");

            var tipoNormalizado = dto.TipoDocumento.Trim().ToUpper();
            if (!TiposRequeridos.Contains(tipoNormalizado))
                throw new ArgumentException($"Tipo de documento inválido: '{dto.TipoDocumento}'.");

            var rutaUrl = await _pdfService.GuardarPdfAsync(dto.Archivo, tipoNormalizado);
            var claveDoc = await _repository.InsertarDocumentoAspirante(rutaUrl, dto.ClaveAspirante, tipoNormalizado);

            return new DocumentoAdjuntadoDTO
            {
                ClaveDocAspirante = claveDoc,
                TipoDocumento = tipoNormalizado,
                RutaUrl = rutaUrl
            };
        }

        public async Task<EstadoDocumentosResponseDTO> ObtenerEstadoDocumentosAsync(string claveAspirante)
        {
            if (string.IsNullOrWhiteSpace(claveAspirante))
                throw new ArgumentException("La clave del aspirante es obligatoria.");

            var docs = await _repository.ObtenerDocumentosConEstatusAsync(claveAspirante);

            var documentosCargados = docs.Select(d => new DocumentoCargadoDTO
            {
                ClaveDocAspirante = d.ClaveDocAspirante,
                TipoDocumento = d.NombreTipoDocumento,
                RutaUrl = d.RutaArchivo,
                Estatus = d.Estatus
            }).ToList();

            return new EstadoDocumentosResponseDTO
            {
                ClaveAspirante = claveAspirante,
                DocumentosCargados = documentosCargados,
                TodosCompletos = docs.Count >= TiposRequeridos.Count
            };
        }

        public async Task<AdjuncionResponseDTO> FinalizarAdjuncionAsync(FinalizarAdjuncionRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveTutor))
                throw new ArgumentException("La clave del tutor es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.ClaveAspirante))
                throw new ArgumentException("La clave del aspirante es obligatoria.");

            var docsPendientes = await _repository.ObtenerDocumentosSinDetalleAsync(dto.ClaveAspirante);

            if (docsPendientes.Count < TiposRequeridos.Count)
                throw new ArgumentException($"Faltan documentos por cargar. Se requieren {TiposRequeridos.Count}, hay {docsPendientes.Count}.");

            var claveAdjuncion = await _repository.CrearAdjuncionAsync(dto.ClaveTutor, dto.ClaveAspirante);
            if (string.IsNullOrWhiteSpace(claveAdjuncion))
                throw new Exception("No se pudo generar la clave de adjunción.");

            var documentosRespuesta = new List<DocumentoAdjuntadoDTO>();
            foreach (var doc in docsPendientes)
            {
                await _repository.InsertarDetalleAdjuncionAsync(claveAdjuncion, doc.ClaveDocAspirante);

                documentosRespuesta.Add(new DocumentoAdjuntadoDTO
                {
                    ClaveDocAspirante = doc.ClaveDocAspirante,
                    TipoDocumento = doc.ClaveTipoDocumento,
                    RutaUrl = doc.RutaArchivo
                });
            }

            var adjuncion = await _repository.ObtenerAdjuncionAsync(claveAdjuncion);

            return new AdjuncionResponseDTO
            {
                ClaveAdjuncion = claveAdjuncion,
                ClaveTutor = dto.ClaveTutor,
                ClaveAspirante = dto.ClaveAspirante,
                EstatusGral = adjuncion?.EstatusGral ?? "Pendiente",
                Documentos = documentosRespuesta
            };
        }

        public async Task<CorregirDocumentoResponseDTO> CorregirDocumentoRechazadoAsync(string claveDocAspirante, IFormFile archivo)
        {
            if (string.IsNullOrWhiteSpace(claveDocAspirante))
                throw new ArgumentException("La clave del documento es obligatoria.");
            if (archivo == null || archivo.Length == 0)
                throw new ArgumentException("El archivo es obligatorio.");
            if (Path.GetExtension(archivo.FileName).ToLower() != ".pdf")
                throw new ArgumentException("El archivo debe ser un PDF.");

            var rutaNueva = await _pdfService.GuardarPdfAsync(archivo, "correccion");
            await _repository.ActualizarRutaDocumentoRechazadoAsync(claveDocAspirante, rutaNueva);

            return new CorregirDocumentoResponseDTO
            {
                ClaveDocAspirante = claveDocAspirante,
                RutaUrl = rutaNueva,
                Mensaje = "Documento corregido correctamente. Pendiente de nueva revisión."
            };
        }

        public async Task<AdjuncionResponseDTO> ReenviarAdjuncionAsync(FinalizarAdjuncionRequestDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ClaveTutor))
                throw new ArgumentException("La clave del tutor es obligatoria.");
            if (string.IsNullOrWhiteSpace(dto.ClaveAspirante))
                throw new ArgumentException("La clave del aspirante es obligatoria.");

            // Obtiene los documentos cuyo último estatus es Rechazado
            var docsRechazados = await _repository.ObtenerDocumentosRechazadosAsync(dto.ClaveAspirante);

            if (!docsRechazados.Any())
                throw new ArgumentException(
                    "No hay documentos rechazados para reenviar. " +
                    "Si aún no tienes una revisión, usa el endpoint de finalizar.");

            // Crea la nueva adjunción — el SP permite esto porque la anterior está Rechazada
            var claveAdjuncion = await _repository.CrearAdjuncionAsync(dto.ClaveTutor, dto.ClaveAspirante);
            if (string.IsNullOrWhiteSpace(claveAdjuncion))
                throw new Exception("No se pudo generar la nueva clave de adjunción.");

            // Vincula cada documento rechazado-corregido a la nueva adjunción
            // El SP del detalle permite esto porque el cuarto filtro histórico
            // solo bloquea documentos en adjunciones Pendiente o Aceptada, no Rechazada
            var documentosRespuesta = new List<DocumentoAdjuntadoDTO>();
            foreach (var doc in docsRechazados)
            {
                await _repository.InsertarDetalleAdjuncionAsync(claveAdjuncion, doc.ClaveDocAspirante);

                documentosRespuesta.Add(new DocumentoAdjuntadoDTO
                {
                    ClaveDocAspirante = doc.ClaveDocAspirante,
                    TipoDocumento = doc.ClaveTipoDocumento,
                    RutaUrl = doc.RutaArchivo
                });
            }

            // El trigger del cuidador ya se encargó de cerrar la adjunción
            // y mandarla a FilaVirtual al insertar el último detalle
            var adjuncion = await _repository.ObtenerAdjuncionAsync(claveAdjuncion);

            return new AdjuncionResponseDTO
            {
                ClaveAdjuncion = claveAdjuncion,
                ClaveTutor = dto.ClaveTutor,
                ClaveAspirante = dto.ClaveAspirante,
                EstatusGral = adjuncion?.EstatusGral ?? "Pendiente",
                Documentos = documentosRespuesta
            };
        }
    }
}
