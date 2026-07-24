CREATE OR REPLACE PROCEDURE sp_insertar_detalle_adjuncion(
    p_claveAdjuncion VARCHAR(18),
    p_claveDocAspirante VARCHAR(18),
    p_motivo_rechazo TEXT DEFAULT NULL -- Único parámetro opcional restante
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_aspirante_adjuncion VARCHAR(18);
    v_aspirante_documento VARCHAR(18);
    v_estatus_previo VARCHAR(20);
    v_adj_estatus_previo VARCHAR(50);
    v_motivo_final TEXT;
    
    -- La constante automática
    v_estatus_automatico VARCHAR(20) := 'Pendiente';
    
    -- Variables para la validación de área y tipo
    v_clave_tipo_doc VARCHAR(18);
    v_area_documento VARCHAR(20);
    v_ya_existe_tipo_en_adj BOOLEAN;
    
    -- Variables para el monitoreo de meta
    v_meta_requisitos INTEGER;
    v_tuplas_actuales INTEGER;
    
    -- Variable para validar estatus actual de la adjunción destino
    v_estatus_adj_destino VARCHAR(50);

	-- Variables añadidas para la validación de ruta histórica
    v_ruta_actual VARCHAR(255);
    v_ultima_ruta_rechazada VARCHAR(255);
BEGIN
    -- Inicializamos el motivo con el valor recibido (aunque será NULL por ser Pendiente)
    v_motivo_final := p_motivo_rechazo;

    -- ============================================================
    -- 1. PRIMER FILTRO: EXISTENCIA Y SEGURIDAD (ENCAPSULAMIENTO)
    -- ============================================================
    -- Verificamos que la Adjunción exista y obtenemos su dueño
    SELECT "claveAspirante", estatus_gral INTO v_aspirante_adjuncion, v_estatus_adj_destino 
    FROM "Adjunciones" 
    WHERE "claveAdjuncion" = p_claveAdjuncion;

    IF v_aspirante_adjuncion IS NULL THEN
        RAISE EXCEPTION 'Error Crítico: La Adjunción % no existe.', p_claveAdjuncion;
    END IF;

    -- Verificamos que el Documento exista y obtenemos su dueño y tipo
    -- AJUSTE AQUÍ: Obtenemos dueño, tipo y también la RUTA ACTUAL del documento
    SELECT "claveAspirante", "claveTipoDocumento", "ruta_archivo" 
    INTO v_aspirante_documento, v_clave_tipo_doc, v_ruta_actual
    FROM "DocumentosAspirante" WHERE "claveDocAspirante" = p_claveDocAspirante;
    
    IF v_aspirante_documento IS NULL THEN
        RAISE EXCEPTION 'Error Crítico: El Documento % no existe.', p_claveDocAspirante;
    END IF;

    -- VALIDACIÓN DE SEGURIDAD: ¿El documento es del mismo aspirante?
    IF v_aspirante_adjuncion <> v_aspirante_documento THEN
        RAISE EXCEPTION 'Bloqueo de Seguridad: El documento % pertenece al aspirante %, no coincide con el dueño de la adjunción %.', 
                        p_claveDocAspirante, v_aspirante_documento, v_aspirante_adjuncion;
    END IF;

    -- ============================================================
    -- 2. SEGUNDO FILTRO: ESTADO DE LA ADJUNCIÓN Y META
    -- ============================================================
    
    -- Bloqueo si la adjunción ya fue evaluada (Aceptada/Rechazada)
    IF v_estatus_adj_destino IN ('Aceptada', 'Rechazada') THEN
        RAISE EXCEPTION 'Bloqueo de Flujo: La adjunción % ya se encuentra % y no puede recibir nuevos documentos.', 
                        p_claveAdjuncion, v_estatus_adj_destino;
    END IF;

    -- AJUSTE REQUERIDO: Contamos la meta considerando solo registros donde estado_requisito sea TRUE
    SELECT COUNT(*) INTO v_meta_requisitos 
    FROM "Requisitos" 
    WHERE etapa_proceso = 'Preinscripción'
      AND estado_requisito = TRUE;

    -- Contamos cuántos lleva
    SELECT COUNT(*) INTO v_tuplas_actuales 
    FROM "DetalleAdjuncion" 
    WHERE "claveAdjuncion" = p_claveAdjuncion;

    -- Bloqueo de meta (Usando tu operador >= para evaluar contra los requisitos válidos)
    IF v_tuplas_actuales >= v_meta_requisitos THEN
        RAISE EXCEPTION 'Bloqueo de Meta: La adjunción % ya cuenta con los % requisitos de Preinscripción.', 
                        p_claveAdjuncion, v_meta_requisitos;
    END IF;

    -- ============================================================
    -- 3. TERCER FILTRO: REGLAS DE ÁREA Y UNICIDAD
    -- ============================================================
    
    -- Validación de Área
    SELECT area INTO v_area_documento FROM "TipoDocumentos" WHERE "claveTipoDocumento" = v_clave_tipo_doc;
    IF v_area_documento <> 'Preinscripción' THEN
        RAISE EXCEPTION 'Bloqueo de Área: El documento es de tipo "%". Solo se permiten documentos de "Preinscripción".', v_area_documento;
    END IF;

    -- NUEVO BLOQUEO: Si pasó el área, asegurar que exista y esté activo en la entidad Requisitos
    IF NOT EXISTS (
        SELECT 1 
        FROM "Requisitos" 
        WHERE "claveTipoDocumento" = v_clave_tipo_doc
          AND "etapa_proceso" = 'Preinscripción'
          AND "estado_requisito" = TRUE
    ) THEN
        RAISE EXCEPTION 'Bloqueo de Requisito: El documento pertenece a Preinscripción, pero NO está catalogado como un requisito activo para este proceso.';
    END IF;

    -- Validación de Unicidad por Tipo
    SELECT EXISTS (
        SELECT 1 FROM "DetalleAdjuncion" da
        JOIN "DocumentosAspirante" doc ON da."claveDocAspirante" = doc."claveDocAspirante"
        WHERE da."claveAdjuncion" = p_claveAdjuncion 
        AND doc."claveTipoDocumento" = v_clave_tipo_doc
    ) INTO v_ya_existe_tipo_en_adj;

    IF v_ya_existe_tipo_en_adj THEN
        RAISE EXCEPTION 'Bloqueo de Duplicidad: Ya existe un documento del tipo "%" en esta adjunción.', v_clave_tipo_doc;
    END IF;

    -- ============================================================
    -- 4. CUARTO FILTRO: HISTORIAL Y NEGOCIO
    -- ============================================================
    
    -- Verificamos si ya existe en un proceso activo o fue aceptado antes
    SELECT da.estatus_documento, a.estatus_gral 
    INTO v_estatus_previo, v_adj_estatus_previo
    FROM "DetalleAdjuncion" da
    JOIN "Adjunciones" a ON da."claveAdjuncion" = a."claveAdjuncion"
    WHERE da."claveDocAspirante" = p_claveDocAspirante
		AND a."claveAdjuncion" <> p_claveAdjuncion  -- ? solo esto se agrega
    ORDER BY a."fecha_envio" DESC LIMIT 1;

    IF v_adj_estatus_previo IN ('Pendiente', 'Aceptada') THEN
        RAISE EXCEPTION 'Bloqueo Histórico: El documento ya está en revisión o ya fue Aceptado.';
    END IF;

    IF v_adj_estatus_previo = 'Rechazada' AND v_estatus_previo = 'Aceptado' THEN
        RAISE EXCEPTION 'Bloqueo Histórico: Este documento ya fue validado como Correcto anteriormente.';
    END IF;

	--  NUEVO AJUSTE: Si viene de un rechazo, obligar a cambiar el archivo físico (ruta distinta)
    IF v_adj_estatus_previo = 'Rechazada' AND v_estatus_previo = 'Rechazado' THEN
        SELECT "ruta_archivo_rechazado" INTO v_ultima_ruta_rechazada
        FROM "RutasRechazadas"
        WHERE "claveDocAspirante" = p_claveDocAspirante
        ORDER BY "fecha_registro" DESC LIMIT 1;

        IF v_ultima_ruta_rechazada IS NOT NULL AND v_ruta_actual = v_ultima_ruta_rechazada THEN
            RAISE EXCEPTION 'Bloqueo Antifraude: No puedes reenviar el archivo. La ruta sigue siendo la misma que te fue rechazada. Sube un documento corregido.';
        END IF;
    END IF;

    -- Como el estatus siempre es Pendiente, forzamos motivo a NULL
    v_motivo_final := NULL;

    -- ============================================================
    -- 5. INSERCIÓN FINAL (Estatus oculto al usuario)
    -- ============================================================
    INSERT INTO "DetalleAdjuncion" (
        "claveAdjuncion",
        "claveDocAspirante",
        estatus_documento,
        motivo_rechazo,
        fecha_evaluacion
    ) VALUES (
        p_claveAdjuncion,
        p_claveDocAspirante,
        v_estatus_automatico, -- Siempre 'Pendiente'
        v_motivo_final,       -- Siempre NULL
        CURRENT_DATE
    );

    RAISE NOTICE 'Éxito: Documento % vinculado como %. (Progreso: % / %)', 
                  p_claveDocAspirante, v_estatus_automatico, (v_tuplas_actuales + 1), v_meta_requisitos;

END;
$$;
