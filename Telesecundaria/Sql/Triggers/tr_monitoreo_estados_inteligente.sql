CREATE OR REPLACE FUNCTION fn_tr_gestion_cierre_inteligente()
RETURNS TRIGGER AS $$
DECLARE
    v_registro RECORD;
    v_total_inscritos INTEGER;
    v_mitad_lote INTEGER;
    v_contador INTEGER := 0;
    v_fecha_cita DATE;
    v_hora_aux TIME := '10:00:00';
BEGIN
    -- ==========================================================
    -- 1. CASO: APERTURA (Programada -> Publicada)
    -- ==========================================================
    IF OLD."estado" = 'Programada' AND NEW."estado" = 'Publicada' THEN
        RAISE NOTICE 'INFO: Convocatoria % activada.', NEW."claveConvocatoria";

    -- ==========================================================
    -- 2. CASO: CIERRE (Publicada -> Cerrada)
    -- ==========================================================
    ELSIF OLD."estado" = 'Publicada' AND NEW."estado" = 'Cerrada' THEN
        
        -- ESCENARIO A: Cierre por Cupo Agotado
        IF NEW."cupo_disponible" <= 0 THEN
            RAISE NOTICE 'AVISO: Cupo agotado. Proceso delegado al cupo límite.';
        
        -- ESCENARIO B: Cierre por Tiempo (Cupo disponible > 0)
        ELSIF NEW."cupo_disponible" > 0 THEN
            
            -- FILTRO CRÍTICO: ¿Hubo actividad?
            -- Si el disponible es igual al máximo, nadie entró.
            IF NEW."cupo_disponible" >= NEW."cupo_maximo" THEN
                RAISE NOTICE 'INFO: Cierre por tiempo. Convocatoria desierta, no se generarán notificaciones.';
            
            ELSE
                -- Si llegamos aquí, es que cupo_disponible < cupo_maximo (SI HAY GENTE)
                
                -- ==========================================================
                -- NUEVA LÓGICA DE DESACTIVACIÓN Y LIMPIEZA (BLOQUE COMPLETO)
                -- ==========================================================

                -- 1.1 DESACTIVAR ASPIRANTES EN FILA (Los que no alcanzaron cupo)
                UPDATE "Aspirantes" 
                SET "estatus_aspirante" = 'Rechazado',
                    "estado" = FALSE
                WHERE "claveAspirante" IN (
                    SELECT "claveAspirante" FROM "FilaVirtual" WHERE "claveConvocatoria" = NEW."claveConvocatoria"
                );

                -- 1.2 DESACTIVACIÓN SELECTIVA DE TUTORES Y RECEPTORES
                -- A. Desactivar Receptores
                UPDATE "Receptores"
                SET "estado" = FALSE
                WHERE "claveTutorAspirante" IN (
                    SELECT DISTINCT asp."claveTutorAspirante" 
                    FROM "Aspirantes" asp
                    INNER JOIN "FilaVirtual" fv ON asp."claveAspirante" = fv."claveAspirante"
                    WHERE fv."claveConvocatoria" = NEW."claveConvocatoria"
                )
                AND NOT EXISTS (
                    SELECT 1 
                    FROM "Aspirantes" a2 
                    WHERE a2."claveTutorAspirante" = "Receptores"."claveTutorAspirante" 
                      AND a2."claveConvocatoria" = NEW."claveConvocatoria"
                      AND a2."estatus_aspirante" = 'Aceptado'
                );

                -- B. Desactivar TutorAspirante
                UPDATE "TutorAspirante"
                SET "estado" = FALSE
                WHERE "claveTutorAspirante" IN (
                    SELECT DISTINCT asp."claveTutorAspirante" 
                    FROM "Aspirantes" asp
                    INNER JOIN "FilaVirtual" fv ON asp."claveAspirante" = fv."claveAspirante"
                    WHERE fv."claveConvocatoria" = NEW."claveConvocatoria"
                )
                AND NOT EXISTS (
                    SELECT 1 
                    FROM "Aspirantes" a2 
                    WHERE a2."claveTutorAspirante" = "TutorAspirante"."claveTutorAspirante" 
                      AND a2."claveConvocatoria" = NEW."claveConvocatoria"
                      AND a2."estatus_aspirante" = 'Aceptado'
                );

                -- 1.3 LIMPIEZA DE FILA VIRTUAL
                DELETE FROM "FilaVirtual" WHERE "claveConvocatoria" = NEW."claveConvocatoria";

                -- BARRIDO DE PENDIENTES Y TUTORES MALDOSOS (AJUSTADO)
                
                -- A. Desactivar Aspirantes que se quedaron en el limbo
                UPDATE "Aspirantes"
                SET "estatus_aspirante" = 'Rechazado',
                    "estado" = FALSE
                WHERE "claveConvocatoria" = NEW."claveConvocatoria"
                  AND "estatus_aspirante" = 'En proceso'
                  AND "estado" = TRUE;

                -- B. Desactivar Tutores de Pendientes solo si no tienen ningún Aceptado
                UPDATE "TutorAspirante"
                SET "estado" = FALSE
                WHERE "estado" = TRUE
                  AND "claveTutorAspirante" IN (
                      SELECT DISTINCT "claveTutorAspirante" 
                      FROM "Aspirantes" 
                      WHERE "claveConvocatoria" = NEW."claveConvocatoria" 
                        AND "estatus_aspirante" = 'Rechazado'
                        AND "estado" = FALSE
                  )
                  AND NOT EXISTS (
                      SELECT 1 FROM "Aspirantes" a3
                      WHERE a3."claveTutorAspirante" = "TutorAspirante"."claveTutorAspirante"
                        AND a3."claveConvocatoria" = NEW."claveConvocatoria"
                        AND a3."estatus_aspirante" = 'Aceptado'
                  );

                -- C. Desactivar Receptores de esos tutores que acabamos de apagar
                UPDATE "Receptores"
                SET "estado" = FALSE
                WHERE "claveTutorAspirante" IN (
                    SELECT "claveTutorAspirante" FROM "TutorAspirante" WHERE "estado" = FALSE
                )
                AND "estado" = TRUE;

                -- ==========================================================
                -- CONTINUACIÓN DE TU LÓGICA ORIGINAL: AGENDAMIENTO
                -- ==========================================================

                -- Contamos solo los que realmente fueron aceptados en revisiones
                SELECT COUNT(*) INTO v_total_inscritos 
                FROM "RevisionesAceptadas" 
                WHERE "claveConvocatoria" = NEW."claveConvocatoria" AND "Estado" = TRUE;

                IF v_total_inscritos > 0 THEN
                    RAISE NOTICE 'LOG: Iniciando agendamiento por tiempo para % aspirantes.', v_total_inscritos;

                    -- LÓGICA DE LOTES Y FAMILIAS
                    v_mitad_lote := v_total_inscritos - (v_total_inscritos / 2);
                    v_fecha_cita := (CURRENT_DATE + INTERVAL '5 days')::DATE;

                    FOR v_registro IN 
                        SELECT ra."claveRevision" 
                        FROM "RevisionesAceptadas" ra
                        INNER JOIN "Revisiones" r ON ra."claveRevision" = r."claveRevision"
                        INNER JOIN "Adjunciones" adj ON r."claveAdjuncion" = adj."claveAdjuncion"
                        INNER JOIN "Aspirantes" asp ON adj."claveAspirante" = asp."claveAspirante"
                        WHERE ra."claveConvocatoria" = NEW."claveConvocatoria" AND ra."Estado" = TRUE
                        ORDER BY asp."claveTutorAspirante" ASC, ra."fecha_aceptacion" ASC
                    LOOP
                        v_contador := v_contador + 1;

                        IF v_contador > v_mitad_lote THEN
                            v_fecha_cita := v_fecha_cita + INTERVAL '1 day';
                            v_mitad_lote := v_total_inscritos;
                            
                            WHILE EXTRACT(DOW FROM v_fecha_cita) IN (0, 6) LOOP
                                v_fecha_cita := v_fecha_cita + INTERVAL '1 day';
                            END LOOP;
                        END IF;

                        CALL sp_agendar_cita_individual(v_registro."claveRevision", v_fecha_cita, v_hora_aux);
                    END LOOP;
                    
                    RAISE NOTICE 'ÉXITO: Citas generadas por cierre de tiempo.';
                ELSE
                    RAISE NOTICE 'INFO: Hubo intentos de registro pero ninguna revisión fue aceptada.';
                END IF;
            END IF;
        END IF;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS tr_monitoreo_estados_inteligente ON "Convocatorias";

CREATE TRIGGER tr_monitoreo_estados_inteligente
AFTER UPDATE OF "estado" ON "Convocatorias"
FOR EACH ROW
EXECUTE FUNCTION fn_tr_gestion_cierre_inteligente();