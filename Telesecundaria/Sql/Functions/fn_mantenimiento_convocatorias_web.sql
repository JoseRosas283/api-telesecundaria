CREATE OR REPLACE FUNCTION fn_mantenimiento_convocatorias_web()
RETURNS void AS $$
BEGIN
    -- ==========================================================
    -- 1. APERTURA SINCRONIZADA (Lógica del "Más Reciente")
    -- ==========================================================
    WITH convocatorias_abiertas AS (
        UPDATE "Convocatorias"
        SET estado = 'Publicada',
            activacion = TRUE
        WHERE estado = 'Programada' 
          AND CURRENT_TIMESTAMP >= fecha_inicio 
          AND CURRENT_TIMESTAMP < fecha_fin
        RETURNING "claveConvocatoria"
    )
    UPDATE "Publicaciones"
    SET estatus_visible = TRUE
    WHERE ("claveConvocatoria", fecha_registro) IN (
        -- Buscamos el registro ganador (el más nuevo) de cada convocatoria abierta
        -- que el Director haya dejado marcado como 'TRUE' en su procedimiento
        SELECT "claveConvocatoria", MAX(fecha_registro)
        FROM "Publicaciones"
        WHERE categoria = 'Convocatorias'
          AND estatus_visible = FALSE  -- Solo tomamos la que está "en espera"
          AND "claveConvocatoria" IN (SELECT "claveConvocatoria" FROM convocatorias_abiertas)
        GROUP BY "claveConvocatoria"
    );
    -- ==========================================================
    -- 2. CIERRE SINCRONIZADO (Apagado masivo por FK)
    -- ==========================================================
    WITH convocatorias_cerradas AS (
        UPDATE "Convocatorias"
        SET estado = 'Cerrada',
            activacion = FALSE
        WHERE (estado = 'Publicada' OR estado = 'Programada') 
          AND CURRENT_TIMESTAMP >= fecha_fin
        RETURNING "claveConvocatoria"
    )
    UPDATE "Publicaciones"
    SET estatus_visible = FALSE
    WHERE categoria = 'Convocatorias'
      -- Aquí no importa la fecha; si la convocatoria cierra, 
      -- apagamos todas las versiones que sigan en TRUE.
      AND estatus_visible = TRUE
      AND "claveConvocatoria" IN (SELECT "claveConvocatoria" FROM convocatorias_cerradas);
END;
$$ LANGUAGE plpgsql;