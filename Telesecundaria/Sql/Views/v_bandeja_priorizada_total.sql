CREATE OR REPLACE VIEW "v_bandeja_priorizada_total" AS
SELECT 
    f.numero_lugar AS "Turno",
    -- Clasificación completa de la fila
    CASE 
        -- 1. Los que vienen de corrección (Sótano/Advertencia)
        WHEN EXISTS (SELECT 1 FROM "Adjunciones" ad2 
                     WHERE ad2."claveAspirante" = asp."claveAspirante" 
                     AND ad2.estatus_gral = 'Rechazada' 
                     AND ad2."claveAdjuncion" <> a."claveAdjuncion") 
             THEN ' REINTENTO: Aspirante con Rechazo Previo'
        
        -- 2. Prioridad por Hermano
        WHEN asp."Hermano_Plantel" = TRUE THEN ' PRIORIDAD: Hermano en Plantel'
        
        -- 3. Discapacidad (Solo los primeros 2 del conteo de la fila)
        WHEN asp.tiene_discapacidad = TRUE AND (
            SELECT COUNT(*) FROM "FilaVirtual" f2 
            JOIN "Aspirantes" asp2 ON f2."claveAspirante" = asp2."claveAspirante"
            WHERE asp2.tiene_discapacidad = TRUE AND f2.numero_lugar <= f.numero_lugar
        ) <= 2 THEN ' PRIORIDAD: Cupo Discapacidad'
        
        -- 4. Aspirante Normal o Excedente
        ELSE 'ESTÁNDAR: Turno por orden de llegada'
    END AS "Tipo_Fila",

    -- Datos de la Notificación y Aspirante
    asp.nombre || ' ' || asp.apellido_paterno AS "Aspirante",
    n.titulo AS "Notificación",
    n.fecha_creacion AS "Enviado",
    a."claveAdjuncion",
    n."claveReceptor"
FROM "Notificaciones" n
INNER JOIN "Adjunciones" a ON (n.datos->>'folio_adjuncion') = a."claveAdjuncion"
INNER JOIN "Aspirantes" asp ON a."claveAspirante" = asp."claveAspirante"
INNER JOIN "FilaVirtual" f ON a."claveAspirante" = f."claveAspirante"
WHERE a.estatus_operativo = 'Cerrada'
ORDER BY f.numero_lugar ASC;