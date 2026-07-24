CREATE OR REPLACE VIEW "vista_detalles_adjuncion_completa" AS
SELECT 
    -- 1. Cabecera de la Adjunción (SÓLO APARECE UNA VEZ )
    a."claveAdjuncion",
    a.fecha_envio AS fecha_envio_adjuncion,
    a.estatus_gral AS estatus_general_adjuncion,
    a.estatus_operativo AS estatus_operativo_adjuncion,

    -- 2. Datos del Aspirante (SÓLO APARECE UNA VEZ )
    asp."claveAspirante",
    asp.curp AS curp_aspirante,
    asp.estatus_aspirante,
    CONCAT(asp.nombre, ' ', asp.apellido_paterno, ' ', COALESCE(asp.apellido_materno, '')) AS aspirante_nombre_completo,

    -- 3. Datos del Tutor (SÓLO APARECE UNA VEZ )
    t."claveTutorAspirante",
    t.curp_tutor,
    t.telefono AS telefono_tutor,
    t.correo AS correo_tutor,
    CONCAT(t.nombre, ' ', t.apellido_paterno, ' ', COALESCE(t.apellido_materno, '')) AS tutor_nombre_completo,

    -- Total de documentos en el lote (Conteo directo)
    COUNT(da."claveDocAspirante") AS total_documentos_lote,

    -- 4. EL LISTADO DE DOCUMENTOS INTEGRADO
    -- Agrupa los 4 documentos en un formato limpio dentro de esta única fila
    JSONB_AGG(
        JSONB_BUILD_OBJECT(
            'pk_documento_aspirante', da."claveDocAspirante",
            'nombre_documento', td.nombre_documento,
            'area_documento', td.area,
            'estatus_documento', da.estatus_documento,
            'motivo_rechazo', da.motivo_rechazo,
            'fecha_evaluacion', da.fecha_evaluacion,
            'ruta_final_documento', CASE 
                WHEN da.estatus_documento = 'Rechazado' THEN hist.ruta_archivo_rechazado
                ELSE doc.ruta_archivo
            END
        ) ORDER BY da."claveDocAspirante"
    ) AS listado_documentos

FROM "Adjunciones" a
INNER JOIN "Aspirantes" asp ON a."claveAspirante" = asp."claveAspirante"
INNER JOIN "TutorAspirante" t ON a."claveTutorAspirante" = t."claveTutorAspirante"
INNER JOIN "DetalleAdjuncion" da ON a."claveAdjuncion" = da."claveAdjuncion"
INNER JOIN "DocumentosAspirante" doc ON da."claveDocAspirante" = doc."claveDocAspirante"
INNER JOIN "TipoDocumentos" td ON doc."claveTipoDocumento" = td."claveTipoDocumento"
LEFT JOIN (
    SELECT DISTINCT ON ("claveAdjuncion", "claveDocAspirante") 
           "claveAdjuncion", 
           "claveDocAspirante", 
           ruta_archivo_rechazado
    FROM "RutasRechazadas"
    ORDER BY "claveAdjuncion", "claveDocAspirante", fecha_registro DESC 
) hist ON da."claveAdjuncion" = hist."claveAdjuncion" AND da."claveDocAspirante" = hist."claveDocAspirante"

--  Agrupamos para colapsar las filas repetidas que veías en tu pantalla
GROUP BY 
    a."claveAdjuncion", 
    asp."claveAspirante", 
    t."claveTutorAspirante";