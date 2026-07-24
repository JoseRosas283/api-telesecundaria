CREATE OR REPLACE FUNCTION fn_cuidador_cierre_automatico()
RETURNS TRIGGER AS $$
DECLARE
    v_claveAspirante VARCHAR(18);
    v_estatus_operativo VARCHAR(20);
    v_ultima_adj_rechazada VARCHAR(18);
    v_total_requisitos_pre INTEGER;
    v_total_entregados_actual INTEGER;
    v_total_rechazados_anterior INTEGER;
    v_meta_a_cumplir INTEGER;
BEGIN
    -- 1. OBTENER INFORMACIÓN DE LA ADJUNCIÓN QUE RECIBIÓ EL DOCUMENTO
    SELECT "claveAspirante", estatus_operativo 
    INTO v_claveAspirante, v_estatus_operativo
    FROM "Adjunciones"
    WHERE "claveAdjuncion" = NEW."claveAdjuncion";

    -- Si la adjunción ya está cerrada, no hacemos nada (evita bucles)
    IF v_estatus_operativo = 'Cerrada' THEN
        RETURN NEW;
    END IF;

    -- 2. CALCULAR LA META BASE (REQUISITOS TOTALES DEL CATÁLOGO)
    SELECT COUNT(*) INTO v_total_requisitos_pre
    FROM "Requisitos"
    WHERE etapa_proceso = 'Preinscripción'
	AND estado_requisito = TRUE ;

    -- 3. BUSCAR SI EL ASPIRANTE TIENE UN RECHAZO PREVIO (PARA LA META DE CORRECCIÓN)
    SELECT "claveAdjuncion" INTO v_ultima_adj_rechazada
    FROM "Adjunciones"
    WHERE "claveAspirante" = v_claveAspirante
      AND estatus_gral = 'Rechazada'
      AND "claveAdjuncion" <> NEW."claveAdjuncion"
    ORDER BY fecha_envio DESC LIMIT 1;

    -- 4. DETERMINAR CUÁNTOS DOCUMENTOS ESTAMOS ESPERANDO (META DINÁMICA)
    IF v_ultima_adj_rechazada IS NULL THEN
        -- Es aspirante nuevo: meta completa
        v_meta_a_cumplir := v_total_requisitos_pre;
    ELSE
        -- Es corrección: contamos cuántos le rechazaron en la carpeta anterior
        SELECT COUNT(*) INTO v_total_rechazados_anterior
        FROM "DetalleAdjuncion"
        WHERE "claveAdjuncion" = v_ultima_adj_rechazada
          AND estatus_documento = 'Rechazado';

        v_meta_a_cumplir := CASE 
            WHEN v_total_rechazados_anterior = 0 THEN v_total_requisitos_pre 
            ELSE v_total_rechazados_anterior 
        END;
    END IF;

    -- 5. CONTAR CUÁNTOS LLEVA EN LA ADJUNCIÓN ACTUAL
    SELECT COUNT(DISTINCT "claveDocAspirante") INTO v_total_entregados_actual
    FROM "DetalleAdjuncion"
    WHERE "claveAdjuncion" = NEW."claveAdjuncion";

    -- ==========================================================
    -- 6. LA LÓGICA DEL CUIDADOR (EL MOMENTO DE LA VERDAD)
    -- ==========================================================
    
    -- Si aún no llega a la meta, el trigger termina aquí sin hacer nada.
    -- El aspirante sigue en "Pendiente" y puede seguir subiendo archivos.
    IF v_total_entregados_actual < v_meta_a_cumplir THEN
        RETURN NEW;
    END IF;

    -- Si llegó a la meta (o por error se pasó), cerramos el paso:
    IF v_total_entregados_actual >= v_meta_a_cumplir THEN
        
        -- A) Sellar la Adjunción
        UPDATE "Adjunciones" 
        SET estatus_operativo = 'Cerrada',
            fecha_envio = CURRENT_TIMESTAMP
        WHERE "claveAdjuncion" = NEW."claveAdjuncion";

        -- B) Limpiar estados para que el administrativo revise de cero
        UPDATE "DetalleAdjuncion" 
        SET estatus_documento = 'Pendiente',
            motivo_rechazo = NULL
        WHERE "claveAdjuncion" = NEW."claveAdjuncion";

        -- C) ¡A la fila! (Llamamos al procedimiento de asignación de lugar)
        CALL sp_formar_aspirante_fila(NEW."claveAdjuncion");

        RAISE NOTICE 'Cuidador: Meta de % alcanzada. Adjunción % cerrada y enviada a fila.', 
                     v_meta_a_cumplir, NEW."claveAdjuncion";
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- 7. CREAR EL TRIGGER
CREATE TRIGGER trg_cuidador_monitoreo_adjuncion
AFTER INSERT ON "DetalleAdjuncion"
FOR EACH ROW
EXECUTE FUNCTION fn_cuidador_cierre_automatico();