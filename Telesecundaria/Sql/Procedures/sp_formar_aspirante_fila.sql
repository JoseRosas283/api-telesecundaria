CREATE OR REPLACE PROCEDURE sp_formar_aspirante_fila(
    p_claveAdjuncion VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_aspirante RECORD;
    v_punto_insercion INTEGER;
    v_conteo_discapacidad_aceptados INTEGER; -- Candado 1: El Histórico Real
    v_ya_formados_vip INTEGER;               -- Candado 2: Los Activos arriba
    v_es_correccion BOOLEAN;
    v_es_sotano BOOLEAN;
BEGIN
    -- 1. OBTENER INFORMACIÓN DEL ASPIRANTE
    SELECT asp."claveConvocatoria", a."claveAspirante", 
           asp."Hermano_Plantel", asp."tiene_discapacidad"
    INTO v_aspirante
    FROM "Adjunciones" a
    JOIN "Aspirantes" asp ON a."claveAspirante" = asp."claveAspirante"
    WHERE a."claveAdjuncion" = p_claveAdjuncion;

    -- 2. VERIFICAR SI ES RECHAZADO PREVIO (CORRECCIÓN)
    SELECT EXISTS (
        SELECT 1 FROM "Adjunciones" 
        WHERE "claveAspirante" = v_aspirante."claveAspirante" 
          AND "estatus_gral" = 'Rechazada'
          AND "claveAdjuncion" <> p_claveAdjuncion
    ) INTO v_es_correccion;

    -- ====================================================================
    -- 3. CONTROL DE CUOTA FLOTANTE ACTUALIZADO (MÁXIMO 2 EN RANGO)
    -- ====================================================================
    -- A) Contamos cuántos discapacitados ya fueron aprobados formalmente
    SELECT COUNT(*) INTO v_conteo_discapacidad_aceptados
    FROM "RevisionesAceptadas" ra
    JOIN "Revisiones" r ON ra."claveRevision" = r."claveRevision"
    JOIN "Adjunciones" adj ON r."claveAdjuncion" = adj."claveAdjuncion"
    JOIN "Aspirantes" asp ON adj."claveAspirante" = asp."claveAspirante"
    WHERE ra."claveConvocatoria" = v_aspirante."claveConvocatoria"
      AND asp."tiene_discapacidad" = TRUE;

    -- B) Contamos cuántos discapacitados activos ocupan la zona de revisión ordinaria
    SELECT COUNT(*) INTO v_ya_formados_vip
    FROM "FilaVirtual" f
    JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
    JOIN "Adjunciones" adj ON adj."claveAspirante" = f."claveAspirante"
    WHERE f."claveConvocatoria" = v_aspirante."claveConvocatoria"
      AND asp."tiene_discapacidad" = TRUE
      AND adj."estatus_gral" <> 'Rechazada'
      AND f."numero_lugar" < (
          SELECT COALESCE(MIN(f2."numero_lugar"), 999999)
          FROM "FilaVirtual" f2
          JOIN "Adjunciones" adj2 ON adj2."claveAspirante" = f2."claveAspirante"
          WHERE adj2."estatus_gral" = 'Rechazada'
      );

    -- Regla de Oro: Al sótano si es corrección OR si ya completamos los 2 espacios del rango activo
    v_es_sotano := v_es_correccion OR 
                   (v_aspirante."tiene_discapacidad" = TRUE AND (v_conteo_discapacidad_aceptados + v_ya_formados_vip) >= 2);

    -- 4. DETERMINAR PUNTO DE INSERCION SEGÚN JERARQUÍA REAL
    IF v_es_sotano THEN
        SELECT COALESCE(MAX("numero_lugar"), 0) + 1 INTO v_punto_insercion
        FROM "FilaVirtual"
        WHERE "claveConvocatoria" = v_aspirante."claveConvocatoria";

    ELSIF v_aspirante."Hermano_Plantel" = TRUE THEN
        -- El MAX solo buscará hermanos arriba del primer registro penalizado del sótano
        -- Y TAMBIÉN ignorará a hermanos que estén en el sótano por cuota de discapacidad rebasada
        SELECT COALESCE(MAX(f."numero_lugar"), 0) + 1 INTO v_punto_insercion
        FROM "FilaVirtual" f
        JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
        WHERE f."claveConvocatoria" = v_aspirante."claveConvocatoria" 
          AND asp."Hermano_Plantel" = TRUE
          -- Candado A: Ignora hermanos en sótano por Rechazo
          AND f."numero_lugar" < (
              SELECT COALESCE(MIN(f2."numero_lugar"), 999999)
              FROM "FilaVirtual" f2
              JOIN "Adjunciones" adj2 ON adj2."claveAspirante" = f2."claveAspirante"
              WHERE adj2."estatus_gral" = 'Rechazada'
          )
          -- Candado B: Ignora hermanos en sótano por Cuota de Discapacidad Agotada (Lugares >= 4)
          AND f."numero_lugar" < (
              SELECT COALESCE(MIN(f3."numero_lugar"), 999999)
              FROM "FilaVirtual" f3
              JOIN "Aspirantes" asp3 ON f3."claveAspirante" = asp3."claveAspirante"
              WHERE f3."claveConvocatoria" = v_aspirante."claveConvocatoria"
                AND asp3."tiene_discapacidad" = TRUE
                AND f3."numero_lugar" >= 4
          );

    ELSE
        -- ====================================================================
        -- CORRECCIÓN DEL ELSE: DETERMINAR LÍMITE INFERIOR DE LA ZONA ACTIVA
        -- ====================================================================
        -- Evaluamos dinámicamente cuál es el primer lugar del sótano real
        SELECT LEAST(
            -- Origen A: El primer rechazado tradicional en la tabla
            (SELECT COALESCE(MIN(f2."numero_lugar"), 999999)
             FROM "FilaVirtual" f2
             JOIN "Adjunciones" adj2 ON adj2."claveAspirante" = f2."claveAspirante"
             WHERE f2."claveConvocatoria" = v_aspirante."claveConvocatoria" 
               AND adj2."estatus_gral" = 'Rechazada'),
            
            -- Origen B: El primer discapacitado enviado al sótano por cuota agotada (Posiciones >= 4)
            (SELECT COALESCE(MIN(f3."numero_lugar"), 999999)
             FROM "FilaVirtual" f3
             JOIN "Aspirantes" asp3 ON f3."claveAspirante" = asp3."claveAspirante"
             WHERE f3."claveConvocatoria" = v_aspirante."claveConvocatoria" 
               AND asp3."tiene_discapacidad" = TRUE 
               AND f3."numero_lugar" >= 4)
        ) INTO v_punto_insercion;

        -- Si el resultado es 999999 significa que el sótano está vacío
        IF v_punto_insercion = 999999 THEN
            SELECT COALESCE(MAX("numero_lugar"), 0) + 1 INTO v_punto_insercion
            FROM "FilaVirtual"
            WHERE "claveConvocatoria" = v_aspirante."claveConvocatoria";
        END IF;
    END IF;

    -- ====================================================================
    -- 5. DESPLAZAMIENTO FÍSICO (CORRECCIÓN CRÍTICA: DESC)
    -- ====================================================================
    SET CONSTRAINTS uq_lugar_convocatoria DEFERRED;

    UPDATE "FilaVirtual"
    SET "numero_lugar" = "numero_lugar" + 1
    WHERE ("claveConvocatoria", "numero_lugar") IN (
        SELECT "claveConvocatoria", "numero_lugar" 
        FROM "FilaVirtual" 
        WHERE "claveConvocatoria" = v_aspirante."claveConvocatoria" 
          AND "numero_lugar" >= v_punto_insercion
        ORDER BY "numero_lugar" DESC
    );

    -- 6. INSERCIÓN FINAL
    INSERT INTO "FilaVirtual" ("claveConvocatoria", "claveAspirante", "numero_lugar")
    VALUES (v_aspirante."claveConvocatoria", v_aspirante."claveAspirante", v_punto_insercion);

    -- 7. SANEAMIENTO DE LA FILA
    WITH Reenumeracion AS (
        SELECT "claveFila", 
               ROW_NUMBER() OVER (ORDER BY "numero_lugar" ASC, "fecha_asignacion" ASC) as lugar_limpio
        FROM "FilaVirtual"
        WHERE "claveConvocatoria" = v_aspirante."claveConvocatoria"
    )
    UPDATE "FilaVirtual" f
    SET "numero_lugar" = r.lugar_limpio
    FROM Reenumeracion r
    WHERE f."claveFila" = r."claveFila";

    RAISE NOTICE 'Aspirante formado dinámicamente. Lugar final: %', v_punto_insercion;

END;
$$;