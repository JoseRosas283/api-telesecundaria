CREATE OR REPLACE FUNCTION fn_tr_verificar_cupo_limite()
RETURNS TRIGGER AS $$
DECLARE
    v_registro RECORD;
    v_total_aceptados INTEGER;
    v_mitad_lote INTEGER;
    v_contador INTEGER := 0;
    v_fecha_cita DATE;
    v_hora_aux TIME := '10:00:00'; -- Hora fija para los lotes
BEGIN
    -- REGLA: Solo si el cupo se agota y estaba activa
    IF NEW.cupo_disponible <= 0 AND OLD.estado = 'Publicada' THEN
        
        -- 1. BLOQUEO DE SEGURIDAD
        UPDATE "Convocatorias" SET estado = 'Cerrada', activacion = FALSE WHERE "claveConvocatoria" = NEW."claveConvocatoria";
        UPDATE "Publicaciones" SET estatus_visible = FALSE WHERE "claveConvocatoria" = NEW."claveConvocatoria";

        -- ==========================================================
        -- NUEVA LÓGICA DE DESACTIVACIÓN Y LIMPIEZA
        -- ==========================================================

        -- 1.1 DESACTIVAR ASPIRANTES EN FILA (Los que no alcanzaron cupo)
        UPDATE "Aspirantes" 
        SET estatus_aspirante = 'Rechazado',
            estado = FALSE
        WHERE "claveAspirante" IN (
            SELECT "claveAspirante" FROM "FilaVirtual" WHERE "claveConvocatoria" = NEW."claveConvocatoria"
        );

        -- 1.2 DESACTIVACIÓN SELECTIVA DE TUTORES Y RECEPTORES
        -- A. Desactivar Receptores
        UPDATE "Receptores"
        SET estado = FALSE
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
              AND a2.estatus_aspirante = 'Aceptado'
        );

        -- B. Desactivar TutorAspirante
        UPDATE "TutorAspirante"
        SET estado = FALSE
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
              AND a2.estatus_aspirante = 'Aceptado'
        );

        -- 1.3 LIMPIEZA DE FILA VIRTUAL
        DELETE FROM "FilaVirtual" WHERE "claveConvocatoria" = NEW."claveConvocatoria";

        -- ==========================================================
        -- NUEVO: BARRIDO DE PENDIENTES Y TUTORES MALDOSOS (AJUSTADO)
        -- ==========================================================
        
        -- A. Desactivar Aspirantes que se quedaron en el limbo (Cambiado 'Pendiente' por 'En proceso')
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
        -- CONTINUACIÓN DE TU LÓGICA ORIGINAL
        -- ==========================================================

        -- 2. CÁLCULO DE LA DIVISIÓN (La lógica de la mitad)
        SELECT COUNT(*) INTO v_total_aceptados 
        FROM "RevisionesAceptadas" 
        WHERE "claveConvocatoria" = NEW."claveConvocatoria" AND Estado = TRUE;

        -- Solo procedemos si hay gente que aceptó para agendar
        IF v_total_aceptados > 0 THEN
            v_mitad_lote := v_total_aceptados - (v_total_aceptados / 2); 

            -- 3. PREPARACIÓN DEL CALENDARIO (Inicia 5 días después)
            v_fecha_cita := (CURRENT_DATE + INTERVAL '5 days')::DATE;

            -- 4. BUCLE DE AGENDACIÓN
            FOR v_registro IN 
                SELECT ra."claveRevision" 
                FROM "RevisionesAceptadas" ra
                INNER JOIN "Revisiones" r ON ra."claveRevision" = r."claveRevision"
                INNER JOIN "Adjunciones" adj ON r."claveAdjuncion" = adj."claveAdjuncion"
                INNER JOIN "Aspirantes" asp ON adj."claveAspirante" = asp."claveAspirante"
                WHERE ra."claveConvocatoria" = NEW."claveConvocatoria" AND ra.Estado = TRUE
                ORDER BY asp."claveTutorAspirante" ASC, ra.fecha_aceptacion ASC
            LOOP
                v_contador := v_contador + 1;

                -- Si ya pasamos la "mitad grande", saltamos al segundo día
                IF v_contador > v_mitad_lote THEN
                    v_fecha_cita := v_fecha_cita + INTERVAL '1 day';
                    v_mitad_lote := v_total_aceptados; 
                    
                    -- Salto de fin de semana si el segundo día cae en sábado
                    WHILE EXTRACT(DOW FROM v_fecha_cita) IN (0, 6) LOOP
                        v_fecha_cita := v_fecha_cita + INTERVAL '1 day';
                    END LOOP;
                END IF;

                -- LLAMADA AL SP ENCAPSULADO
                CALL sp_agendar_cita_individual(v_registro."claveRevision", v_fecha_cita, v_hora_aux);

            END LOOP;
        END IF;

        RAISE NOTICE 'Cierre completo. % aceptados organizados. Limpieza de pendientes y tutores maldosos finalizada.', v_total_aceptados;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS tr_monitorear_cupo ON "Convocatorias";

CREATE TRIGGER tr_monitorear_cupo
AFTER UPDATE OF cupo_disponible ON "Convocatorias"
FOR EACH ROW
EXECUTE FUNCTION fn_tr_verificar_cupo_limite();