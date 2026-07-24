CREATE OR REPLACE FUNCTION tg_cuidador_expediente_completo()
RETURNS TRIGGER AS $$
DECLARE
    v_limite_requisitos INT;
    v_tuplas_actuales INT;
    v_claveEntrega VARCHAR(18);
    v_registro_documento RECORD; -- Para iterar sobre cada documento del detalle
BEGIN
    -- ============================================================
    -- 1. CONTABILIZAR LAS REGLAS DEL JUEGO
    -- ============================================================
    -- Averiguamos cuántos requisitos activos exige la institución para Inscripción
    SELECT COUNT(*) INTO v_limite_requisitos
    FROM "Requisitos"
    WHERE "etapa_proceso" = 'Inscripción'
      AND "estado_requisito" = TRUE;

    -- Contamos cuántas tuplas tiene YA esta adjunción (incluyendo la que se acaba de insertar)
    SELECT COUNT(*) INTO v_tuplas_actuales
    FROM "DetalleAdjuncionOriginal"
    WHERE "claveAdjOriginal" = NEW."claveAdjOriginal";

    -- ============================================================
    -- 2. EVALUAR SI ES VERDAD QUE SE COMPLETÓ EL EXPEDIENTE
    -- ============================================================
    IF v_tuplas_actuales = v_limite_requisitos THEN
        
        -- Viajamos a la tabla maestra para extraer la entrega vinculada a esta adjunción
        SELECT "claveEntrega" INTO v_claveEntrega
        FROM "AdjuncionesOriginales"
        WHERE "claveAdjOriginal" = NEW."claveAdjOriginal";

        -- ============================================================
        -- 3. EL VIAJE POR CADA TUPLA: INSERCIÓN INDIVIDUAL EN VALIDACIÓN
        -- ============================================================
        -- Recorremos todos los documentos que el alumno ya subió para esta adjunción
        FOR v_registro_documento IN 
            SELECT "claveDocAspirante" 
            FROM "DetalleAdjuncionOriginal" 
            WHERE "claveAdjOriginal" = NEW."claveAdjOriginal"
        LOOP
            -- Invocamos tu procedimiento almacenado para procesar cada documento
            -- Tu SP se encargará de verificar la existencia, el candado inter-entrega y el 'Original' por defecto
            CALL sp_cotejar_documento_fisico(v_claveEntrega, v_registro_documento."claveDocAspirante");
        END LOOP;

        -- ============================================================
        -- 4. ACTUALIZACIÓN AUTOMÁTICA DE LA ENTREGA GLOBAL
        -- ============================================================
        UPDATE "Entregas"
        SET "estado_final" = 'Completada'
        WHERE "claveEntrega" = v_claveEntrega;

        RAISE NOTICE 'Cuidador: Se ejecutó con éxito el bulto de validación física para la entrega %.', v_claveEntrega;

    ELSE
        -- Si todavía no tiene las tuplas correctas, no hace nada y el flujo sigue normal
        NULL;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_cuidador_despues_insertar ON "DetalleAdjuncionOriginal";

-- ============================================================
-- 4. CREACIÓN DEL TRIGGER ASOCIADO A LA TABLA DETALLE
-- ============================================================
CREATE OR REPLACE TRIGGER trg_cuidador_despues_insertar
AFTER INSERT ON "DetalleAdjuncionOriginal"
FOR EACH ROW
EXECUTE FUNCTION tg_cuidador_expediente_completo();