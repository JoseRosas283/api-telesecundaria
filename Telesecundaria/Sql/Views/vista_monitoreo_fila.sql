CREATE OR REPLACE VIEW "vista_monitoreo_fila" AS
SELECT 
    f.numero_lugar AS "Posición",
    -- Concatenamos nombre completo
    (asp.nombre || ' ' || asp.apellido_paterno || ' ' || asp.apellido_materno) AS "Nombre Completo",
    -- Traducción de Hermanos
    CASE 
        WHEN asp."Hermano_Plantel" = TRUE THEN 'Cuenta con hermano'
        ELSE 'No cuenta con hermano'
    END AS "Referencia Familiar",
    -- Traducción de Discapacidad
    CASE 
        WHEN asp.tiene_discapacidad = TRUE THEN 'SÍ'
        ELSE 'NO'
    END AS "Discapacidad",
    -- Detectar si es una corrección (Rechazado previo)
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM "Adjunciones" adj 
            WHERE adj."claveAspirante" = f."claveAspirante" 
              AND adj.estatus_gral = 'Rechazada'
        ) THEN 'CORRECCIÓN (Sótano)'
        ELSE 'Original'
    END AS "Origen de Trámite",
    f.fecha_asignacion AS "Fecha de Registro",
    f."claveConvocatoria"
FROM "FilaVirtual" f
JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
ORDER BY f.numero_lugar ASC;