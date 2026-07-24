CREATE OR REPLACE PROCEDURE cerrar_revision_final(
    p_claveRevision VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveAdjuncion VARCHAR(18);
    v_total_esperado INTEGER;
    v_total_revisado INTEGER;
    v_conteo_rechazados INTEGER;
    v_nuevo_estatus VARCHAR(50);
	v_estado_operativo_actual VARCHAR(20); -- Variable corregida para evitar sobreescribir estados
    v_existe_revision BOOLEAN; 

	-- ?? Variable tipo registro para iterar los documentos rechazados uno a uno
    v_fila_rechazada RECORD; 
BEGIN
    -- 1. VALIDACIÓN DE EXISTENCIA FÍSICA
    SELECT EXISTS(SELECT 1 FROM "Revisiones" WHERE "claveRevision" = p_claveRevision) INTO v_existe_revision;
    
    IF NOT v_existe_revision THEN
        RAISE EXCEPTION 'Error: La revisión % no existe en el sistema.', p_claveRevision;
    END IF;

    -- 2. VALIDACIÓN DE CONTENIDO (No cerrar algo vacío)
    SELECT COUNT(*) INTO v_total_revisado 
    FROM "DetalleRevision" 
    WHERE "claveRevision" = p_claveRevision;

    IF v_total_revisado = 0 THEN
        RAISE EXCEPTION 'Error: No se puede cerrar la revisión % porque no tiene documentos en detalleRevision.', p_claveRevision;
    END IF;

    -- 3. Obtener la adjunción vinculada y verificar que la revisión esté ABIERTA
    SELECT "claveAdjuncion", "estado_operativo" 
    INTO v_claveAdjuncion, v_estado_operativo_actual 
    FROM "Revisiones" 
    WHERE "claveRevision" = p_claveRevision;

    IF v_estado_operativo_actual = 'Cerrada' THEN
        RAISE EXCEPTION 'La revisión % ya se encuentra cerrada.', p_claveRevision;
    END IF;

    -- 4. VALIDACIÓN DE COMPLETITUD (Tu lógica original)
    SELECT COUNT(*) INTO v_total_esperado 
    FROM "DetalleAdjuncion" 
    WHERE "claveAdjuncion" = v_claveAdjuncion;

    -- v_total_revisado ya lo tenemos arriba
    IF v_total_esperado <> v_total_revisado THEN
        RAISE EXCEPTION 'No se puede cerrar: Faltan documentos por revisar (% de % revisados).', 
            v_total_revisado, v_total_esperado;
    END IF;

    -- 5. DETERMINAR EL VEREDICTO
    SELECT COUNT(*) INTO v_conteo_rechazados
    FROM "DetalleRevision"
    WHERE "claveRevision" = p_claveRevision AND "estatus_doc" = 'Rechazado';

    IF v_conteo_rechazados > 0 THEN
        v_nuevo_estatus := 'Rechazada';
    ELSE
        v_nuevo_estatus := 'Aceptada';
    END IF;

    -- 6. ACTUALIZACIÓN EN CASCADA (El Espejo)
    
    -- A. Actualizar DetalleAdjuncion (Espejo de DetalleRevision)
    -- Cambio: Se usa 'estatus_documento' en lugar de 'estatus_doc'
    UPDATE "DetalleAdjuncion" da
    SET "estatus_documento" = dr."estatus_doc",
        "motivo_rechazo" = dr."motivo_rechazo", -- Aprovechamos para espejear el motivo
        "fecha_evaluacion" = CURRENT_DATE
    FROM "DetalleRevision" dr
    WHERE da."claveDocAspirante" = dr."claveDocAspirante"
      AND da."claveAdjuncion" = v_claveAdjuncion
      AND dr."claveRevision" = p_claveRevision;

    -- B. Actualizar la Adjunción (Estatus General)
    UPDATE "Adjunciones" 
    SET "estatus_gral" = v_nuevo_estatus 
    WHERE "claveAdjuncion" = v_claveAdjuncion;

    -- C. Cerrar la Revisión (Cabecera)
    -- RECUERDA: Si el TRIGGER que sigue a este UPDATE falla, TODO lo anterior hace ROLLBACK.
    UPDATE "Revisiones" 
    SET "estatus_revision" = v_nuevo_estatus,
        "estado_operativo" = 'Cerrada',
        "fecha_revision" = CURRENT_TIMESTAMP
    WHERE "claveRevision" = p_claveRevision;

	-- =================================================================
    -- ?? 7. INICIALIZACIÓN DEL RESPALDO HISTÓRICO (Solo si fue Rechazada)
    -- =================================================================
    IF v_nuevo_estatus = 'Rechazada' THEN
        
        -- Buscamos cada tupla que se acaba de guardar como 'Rechazado'
        FOR v_fila_rechazada IN (
            SELECT "claveDocAspirante" 
            FROM "DetalleRevision" 
            WHERE "claveRevision" = p_claveRevision AND "estatus_doc" = 'Rechazado'
        ) LOOP
            
            -- Mandamos llamar al procedimiento validador por cada documento individual
            CALL validar_e_insertar_ruta_rechazada(
                v_claveAdjuncion, 
                p_claveRevision, 
                v_fila_rechazada."claveDocAspirante"
            );
            
        END LOOP;
        
    END IF;

    RAISE NOTICE 'Proceso completado. Revisión % y Adjunción % marcadas como %.', 
        p_claveRevision, v_claveAdjuncion, v_nuevo_estatus;

END;
$$;