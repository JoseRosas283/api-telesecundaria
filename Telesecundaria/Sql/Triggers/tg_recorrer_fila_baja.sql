CREATE OR REPLACE FUNCTION fn_recorrer_lugares_fila()
RETURNS TRIGGER AS $$
DECLARE
    v_siguiente_sotano VARCHAR(18);
    v_conteo_discapacidad_aceptados INTEGER;
    v_ya_formados_vip INTEGER;
    v_era_rechazado_discapacitado BOOLEAN;
    v_punto_insercion_comunes INTEGER;
    v_inicio_sotano_real INTEGER;       -- Variable clave para hallar la frontera real
    
    -- Variables para la estrategia de evaluación del candidato del sótano
    v_ganador_tiene_hermano BOOLEAN;
    v_ganador_fecha TIMESTAMP;
    v_lugar_insercion_justo INTEGER;
    v_existen_comunes_activos BOOLEAN;
BEGIN
    -- ??? PASO DE SEGURIDAD INTERNO: Multiplicamos por 1000 para dispersar la fila
    -- Esto elimina de raíz cualquier posibilidad de error 23505 durante el proceso
    UPDATE "FilaVirtual" 
    SET "numero_lugar" = "numero_lugar" * 1000 
    WHERE "claveConvocatoria" = OLD."claveConvocatoria";

    -- ====================================================================
    -- PASO 1: REORDENAR LUGARES INDEPENDIENTEMENTE DE LO QUE PASE (Tu Lógica Base)
    -- ====================================================================
    WITH Reenumeracion AS (
        SELECT 
            "claveFila", 
            ROW_NUMBER() OVER (ORDER BY "numero_lugar" ASC, "fecha_asignacion" ASC) as lugar_limpio
        FROM "FilaVirtual"
        WHERE "claveConvocatoria" = OLD."claveConvocatoria"
    )
    UPDATE "FilaVirtual" f
    SET "numero_lugar" = r.lugar_limpio
    FROM Reenumeracion r
    WHERE f."claveFila" = r."claveFila";

    -- ====================================================================
    -- PASO 2: VERIFICAR SI EL ELIMINADO ERA DISCAPACITADO Y RECHAZADO (Tu flujo)
    -- ====================================================================
    SELECT EXISTS (
        SELECT 1 
        FROM "Aspirantes" asp
        WHERE asp."claveAspirante" = OLD."claveAspirante"
          AND asp."tiene_discapacidad" = TRUE        
          AND asp."estatus_aspirante" = 'Rechazado'  
          AND asp."claveConvocatoria" = OLD."claveConvocatoria" 
    ) INTO v_era_rechazado_discapacitado;

    -- SI NO CUMPLE TUS CONDICIONES, AQUÍ TERMINA TODO
    IF v_era_rechazado_discapacitado = TRUE THEN

        -- ====================================================================
        -- NUEVO: CALCULAR LA FRONTERA REAL DEL SÓTANO MIXTO
        -- ====================================================================
        SELECT LEAST(
            -- Origen A: El primer rechazado en fila (si existe)
            (SELECT COALESCE(MIN(f2."numero_lugar"), 999999)
             FROM "FilaVirtual" f2
             JOIN "Aspirantes" asp2 ON asp2."claveAspirante" = f2."claveAspirante"
             WHERE f2."claveConvocatoria" = OLD."claveConvocatoria" 
               AND asp2."estatus_aspirante" = 'Rechazado'),
            
            -- Origen B: El primer discapacitado enviado al sótano por cuota (Lugar >= 4)
            (SELECT COALESCE(MIN(f3."numero_lugar"), 999999)
             FROM "FilaVirtual" f3
             JOIN "Aspirantes" asp3 ON f3."claveAspirante" = asp3."claveAspirante"
             WHERE f3."claveConvocatoria" = OLD."claveConvocatoria" 
               AND asp3."tiene_discapacidad" = TRUE 
               AND f3."numero_lugar" >= 4)
        ) INTO v_inicio_sotano_real;

        -- ====================================================================
        -- PASO 3: VERIFICAR MEMORIA DE CUPOS (REVISIONES ACEPTADAS < 2)
        -- ====================================================================
        SELECT COUNT(*) INTO v_conteo_discapacidad_aceptados
        FROM "RevisionesAceptadas" ra
        JOIN "Revisiones" r ON ra."claveRevision" = r."claveRevision"
        JOIN "Adjunciones" adj ON r."claveAdjuncion" = adj."claveAdjuncion"
        JOIN "Aspirantes" asp ON adj."claveAspirante" = asp."claveAspirante"
        WHERE ra."claveConvocatoria" = OLD."claveConvocatoria"
          AND asp."tiene_discapacidad" = TRUE;

        -- Contamos los activos arriba de la frontera del sótano real
        SELECT COUNT(*) INTO v_ya_formados_vip
        FROM "FilaVirtual" f
        JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
        WHERE f."claveConvocatoria" = OLD."claveConvocatoria"
          AND asp."tiene_discapacidad" = TRUE
          AND f."numero_lugar" < v_inicio_sotano_real;

        -- El candado de oro: Comienza el juego si hay cupo disponible
        IF (v_conteo_discapacidad_aceptados + v_ya_formados_vip) < 2 THEN
            
            -- ====================================================================
            -- PASO 4: EVALUAR AL SIGUIENTE EN EL SÓTANO (HERMANO > FECHA LONGEVA)
            -- ====================================================================
            SELECT f."claveAspirante" INTO v_siguiente_sotano
            FROM "FilaVirtual" f
            JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
            WHERE f."claveConvocatoria" = OLD."claveConvocatoria"
              AND asp."tiene_discapacidad" = TRUE
              AND f."numero_lugar" >= v_inicio_sotano_real
            ORDER BY 
                CASE WHEN asp."Hermano_Plantel" = TRUE THEN 0 ELSE 1 END ASC,
                f."fecha_asignacion" ASC
            LIMIT 1;

            -- ====================================================================
            -- PASO 5: EL ASCENSOR MATEMÁTICO ADAPTATIVO
            -- ====================================================================
            IF v_siguiente_sotano IS NOT NULL THEN
                
                SELECT asp."Hermano_Plantel", f."fecha_asignacion" 
                INTO v_ganador_tiene_hermano, v_ganador_fecha
                FROM "FilaVirtual" f
                JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
                WHERE f."claveAspirante" = v_siguiente_sotano;

                SELECT EXISTS (
                    SELECT 1 
                    FROM "FilaVirtual" f
                    JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
                    WHERE f."claveConvocatoria" = OLD."claveConvocatoria"
                      AND f."numero_lugar" < v_inicio_sotano_real
                      AND asp."Hermano_Plantel" = FALSE
                ) INTO v_existen_comunes_activos;

                IF v_existen_comunes_activos = TRUE THEN
                    IF v_ganador_tiene_hermano = TRUE THEN
                        SELECT COALESCE(MIN(f."numero_lugar"), 1) INTO v_punto_insercion_comunes
                        FROM "FilaVirtual" f
                        JOIN "Aspirantes" asp ON asp."claveAspirante" = f."claveAspirante"
                        WHERE f."claveConvocatoria" = OLD."claveConvocatoria"
                          AND f."numero_lugar" < v_inicio_sotano_real
                          AND asp."Hermano_Plantel" = FALSE;
                          
                        v_lugar_insercion_justo := v_punto_insercion_comunes - 1;
                    ELSE
                        SELECT MIN(f."numero_lugar") INTO v_lugar_insercion_justo
                        FROM "FilaVirtual" f
                        JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
                        WHERE f."claveConvocatoria" = OLD."claveConvocatoria"
                          AND f."numero_lugar" < v_inicio_sotano_real
                          AND asp."Hermano_Plantel" = FALSE
                          AND f."fecha_asignacion" > v_ganador_fecha;

                        IF v_lugar_insercion_justo IS NULL THEN
                            v_lugar_insercion_justo := v_inicio_sotano_real;
                        END IF;
                    END IF;
                ELSE
                    SELECT COALESCE(MAX(f."numero_lugar"), 0) INTO v_lugar_insercion_justo
                    FROM "FilaVirtual" f
                    JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
                    WHERE f."claveConvocatoria" = OLD."claveConvocatoria"
                      AND f."numero_lugar" < v_inicio_sotano_real
                      AND asp."Hermano_Plantel" = TRUE;

                    IF v_lugar_insercion_justo = 0 THEN
                        v_lugar_insercion_justo := 1;
                    END IF;
                END IF;

                -- ??? SEGUNDO PASO DE SEGURIDAD: Multiplicamos toda la tabla otra vez por 1000 
                -- antes de asignarle la posición exacta a Santiago.
                UPDATE "FilaVirtual" 
                SET "numero_lugar" = "numero_lugar" * 1000 
                WHERE "claveConvocatoria" = OLD."claveConvocatoria";

                -- Insertamos a Santiago restándole 1 a su equivalente multiplicado 
                -- para asegurar que quede por encima del común exacto sin chocar.
                UPDATE "FilaVirtual"
                SET "numero_lugar" = CASE 
                    WHEN "claveAspirante" = v_siguiente_sotano THEN (v_lugar_insercion_justo * 1000) - 1
                    ELSE "numero_lugar" 
                END
                WHERE "claveConvocatoria" = OLD."claveConvocatoria";

                -- Saneamiento final: Regresa la escala de toda la fila a limpio (1, 2, 3...)
                WITH ReenumeracionFinal AS (
                    SELECT 
                        "claveFila", 
                        ROW_NUMBER() OVER (ORDER BY "numero_lugar" ASC, "fecha_asignacion" ASC) as lugar_limpio
                    FROM "FilaVirtual"
                    WHERE "claveConvocatoria" = OLD."claveConvocatoria"
                )
                UPDATE "FilaVirtual" f
                SET "numero_lugar" = r.lugar_limpio
                FROM ReenumeracionFinal r
                WHERE f."claveFila" = r."claveFila";

            END IF;
        END IF;
    END IF;

    RETURN OLD;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER tg_recorrer_fila_baja
AFTER DELETE ON "FilaVirtual"
FOR EACH ROW -- Se ejecuta por cada aspirante que sale (Aceptado o Rechazado)
EXECUTE FUNCTION fn_recorrer_lugares_fila();