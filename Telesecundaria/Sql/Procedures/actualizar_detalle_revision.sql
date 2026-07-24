CREATE OR REPLACE PROCEDURE actualizar_detalle_revision(
    p_claveRevision VARCHAR(18),
    p_claveDocAspirante VARCHAR(18),
    p_estatus_doc VARCHAR(50),
    p_motivo_rechazo TEXT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_estatus_doc_actual VARCHAR(50);
    v_estado_operativo VARCHAR(20);
    
    -- Variable tipo RECORD para capturar la tupla actual completa del detalle de revisión
    v_det_actual RECORD;
BEGIN
    -- =================================================================
    -- 1. VERIFICACIÓN DE EXISTENCIA FÍSICA POR PK COMPUESTA
    -- =================================================================
    -- Modificado para obtener la tupla completa (*) y alimentar el filtro ROW
    SELECT * INTO v_det_actual
    FROM "DetalleRevision" 
    WHERE "claveRevision" = p_claveRevision 
      AND "claveDocAspirante" = p_claveDocAspirante;

    -- Si no existe la fila en la tabla, se detiene de inmediato
    IF v_det_actual."claveRevision" IS NULL THEN
        RAISE EXCEPTION 'Error de Modificación: No existe un registro previo para el documento % en la revisión %. Use el procedimiento de registro en su lugar.', 
            p_claveDocAspirante, p_claveRevision;
    END IF;

    -- Asignación de tu variable de control original a partir del registro obtenido
    v_estatus_doc_actual := v_det_actual.estatus_doc;

    -- =================================================================
    -- 2. VERIFICACIÓN DEL ESTADO OPERATIVO (CANDADO DE CIERRE - NUEVO ORDEN)
    -- =================================================================
    SELECT "estado_operativo" INTO v_estado_operativo
        FROM "Revisiones"
        WHERE "claveRevision" = p_claveRevision;

    IF v_estado_operativo = 'Cerrada' THEN
        RAISE EXCEPTION 'Bloqueo Operativo: No se puede modificar el documento %. La revisión % ya se encuentra CERRADA.', 
            p_claveDocAspirante, p_claveRevision;
    END IF;

    -- =========================================================================
    -- 3. FILTRO DE CAMBIOS REALES (AÑADIDO BAJO TU ESTRATEGIA UNIFICADA)
    -- =========================================================================
    -- Evaluamos simultáneamente el estatus del documento y el motivo de rechazo (limpio)
    IF ROW(v_det_actual.estatus_doc, v_det_actual.motivo_rechazo)
       IS NOT DISTINCT FROM
       ROW(p_estatus_doc, NULLIF(TRIM(p_motivo_rechazo), ''))
    THEN
        -- Indica éxito al cliente/API, deteniendo la ejecución sin gastar recursos en procesos posteriores
        RAISE NOTICE 'Éxito: Operación completada. Los datos enviados para el documento % en la revisión % son idénticos a los actuales; no se realizaron cambios en la base de datos.', 
            p_claveDocAspirante, p_claveRevision;
        RETURN; -- Detiene la ejecución limpia e inmediata antes de procesar el resto de la lógica
    END IF;

    -- =================================================================
    -- 4. VALIDACIÓN DE RECHAZADO: MOTIVO OBLIGATORIO
    -- =================================================================
    -- Si el estado cambió a 'Rechazado' (o ya era y modificaron el motivo) y viene vacío, se frena de inmediato
    IF p_estatus_doc = 'Rechazado' AND (p_motivo_rechazo IS NULL OR TRIM(p_motivo_rechazo) = '') THEN
        RAISE EXCEPTION 'Regla de Negocio: Debe especificar obligatoriamente el motivo de rechazo para el documento %.', 
            p_claveDocAspirante;
    END IF;

    -- =================================================================
    -- 5. VALIDACIÓN DE ACEPTADO: MOTIVO DEBE SER VACÍO O NULL
    -- =================================================================
    -- Si el estado cambió a 'Aceptado' pero intentan inyectar o dejar un motivo escrito, la base de datos lo rebota
    IF p_estatus_doc = 'Aceptado' AND (p_motivo_rechazo IS NOT NULL AND TRIM(p_motivo_rechazo) <> '') THEN
        RAISE EXCEPTION 'Regla de Negocio: Un documento ''Aceptado'' no puede contener un motivo de rechazo. El campo debe venir NULL o vacío.';
    END IF;

    -- =================================================================
    -- 6. ACTUALIZACIÓN DIRECTA DE LOS CAMPOS
    -- =================================================================
    UPDATE "DetalleRevision" SET
        "estatus_doc" = p_estatus_doc,
        -- Guardamos el valor directo. Usamos NULLIF por si el frontend mandó texto vacío para 'Aceptado'
        "motivo_rechazo" = NULLIF(TRIM(p_motivo_rechazo), '')
    WHERE "claveRevision" = p_claveRevision 
      AND "claveDocAspirante" = p_claveDocAspirante;

    RAISE NOTICE 'Éxito: Estatus del documento % actualizado correctamente de % a % en la revisión %.', 
        p_claveDocAspirante, v_estatus_doc_actual, p_estatus_doc, p_claveRevision;

END;
$$;
