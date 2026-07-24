CREATE OR REPLACE PROCEDURE sp_registrar_detalle_adjuncion_original(
    p_claveAdjOriginal VARCHAR(18),
    p_claveDocAspirante VARCHAR(18)
)
AS $$
DECLARE
    v_existe_maestro BOOLEAN;
    v_doc_ya_usado VARCHAR(18);
    v_ruta_ya_usada TEXT;
    
    -- Variables para validar que no se crucen los Aspirantes
    v_aspirante_entrega VARCHAR(18);
    v_aspirante_documento VARCHAR(18);

    -- Variable para recuperar el estado de la entrega
    v_estado_entrega VARCHAR(30);

    -- Flag para verificar la existencia del documento y extraer la ruta automáticamente
    v_existe_documento BOOLEAN;
    v_ruta_automatica TEXT; -- Almacenará la ruta extraída automáticamente

    -- Variables para la validación de catálogos y requisitos
    v_claveTipoDocumento VARCHAR(18);
    v_area_documento VARCHAR(20);
    v_nombre_documento VARCHAR(50);
    v_existe_en_requisitos_inscripcion BOOLEAN;
    v_existe_en_requisitos_preinscripcion BOOLEAN;

    -- Variables para tu regla de igualdad exacta
    v_limite_requisitos INT;
    v_tuplas_actuales INT;
BEGIN

    -- ============================================================
    -- 3. VALIDACIÓN DE EXISTENCIA Y EXTRACCIÓN AUTOMÁTICA DE RUTA
    -- ============================================================
    -- Extraemos la información del documento y recuperamos de forma nativa su ruta de archivo
    SELECT 
        da."claveDocAspirante" IS NOT NULL,
        da."claveAspirante", 
        da."ruta_archivo", -- <-- EXTRACCIÓN AUTOMÁTICA: Campo nativo de la tabla
        td."claveTipoDocumento", 
        td."area", 
        td."nombre_documento"
    INTO 
        v_existe_documento,
        v_aspirante_documento, 
        v_ruta_automatica, -- <-- Almacenada internamente
        v_claveTipoDocumento, 
        v_area_documento, 
        v_nombre_documento
    FROM "DocumentosAspirante" da
    INNER JOIN "TipoDocumentos" td ON da."claveTipoDocumento" = td."claveTipoDocumento"
    WHERE da."claveDocAspirante" = p_claveDocAspirante;

    IF v_existe_documento IS NOT TRUE THEN
        RAISE EXCEPTION 'Error de existencia: El documento con clave % no se encuentra registrado en el sistema.', p_claveDocAspirante;
    END IF;

    -- 1. VALIDACIÓN DE TEXTO: Evaluamos la ruta autodetectada
    IF v_ruta_automatica IS NULL OR trim(v_ruta_automatica) = '' THEN
        RAISE EXCEPTION 'Error de archivo: El documento de origen % existe, pero su campo "ruta_archivo" está vacío.', p_claveDocAspirante;
    END IF;

    -- ============================================================
    -- 2. VALIDACIÓN DE DUPLICIDAD DE RUTA FÍSICA
    -- ============================================================
    SELECT "claveAdjOriginal" INTO v_ruta_ya_usada
    FROM "DetalleAdjuncionOriginal"
    WHERE "ruta_pdf_original" = v_ruta_automatica; -- Comparamos contra la ruta extraída

    IF v_ruta_ya_usada IS NOT NULL THEN
        RAISE EXCEPTION 'Error de almacenamiento: La ruta física "%" ya pertenece a la adjunción %.', 
            v_ruta_automatica, v_ruta_ya_usada;
    END IF;

    -- ============================================================
    -- 4. VALIDACIÓN MAESTRA: Verificar existencia de la Adjunción y ESTADO
    -- ============================================================
    SELECT ao."claveAdjOriginal" IS NOT NULL, e."claveAspirante", e."estado_final" -- Ajustado a estado_final
    INTO v_existe_maestro, v_aspirante_entrega, v_estado_entrega
    FROM "AdjuncionesOriginales" ao
    INNER JOIN "Entregas" e ON ao."claveEntrega" = e."claveEntrega"
    WHERE ao."claveAdjOriginal" = p_claveAdjOriginal;

    IF NOT v_existe_maestro THEN
        RAISE EXCEPTION 'Error de clave: La adjunción original maestra (%) no existe.', p_claveAdjOriginal;
    END IF;

    IF UPPER(TRIM(v_estado_entrega)) = 'COMPLETADA' THEN -- Ajustado el valor de comparación a mayúsculas por el UPPER
        RAISE EXCEPTION 'Bloqueo de seguridad: No se permiten más modificaciones. La entrega vinculada a la adjunción % ya está COMPLETADA.', p_claveAdjOriginal;
    END IF;

    -- ============================================================
    -- 5. VALIDACIÓN DE IDENTIDAD: Comprobar encapsulamiento del Aspirante
    -- ============================================================
    IF v_aspirante_entrega <> v_aspirante_documento THEN
        RAISE EXCEPTION 'Error de identidad: El documento % pertenece al aspirante %, pero la entrega actual es del aspirante %. ¡No se pueden cruzar expedientes!', 
            p_claveDocAspirante, v_aspirante_documento, v_aspirante_entrega;
    END IF;

    -- 6. VALIDACIÓN DE REQUISITOS Y DOBLE CATÁLOGO
    IF v_area_documento = 'Inscripción' THEN
        SELECT EXISTS (
            SELECT 1 FROM "Requisitos"
            WHERE "claveTipoDocumento" = v_claveTipoDocumento 
              AND "etapa_proceso" = 'Inscripción'
              AND "estado_requisito" = TRUE 
        ) INTO v_existe_en_requisitos_inscripcion;

        IF NOT v_existe_en_requisitos_inscripcion THEN
            RAISE EXCEPTION 'Error de catálogo: El documento "%" es de área Inscripción pero no está configurado como Requisito de este proceso.', 
                v_nombre_documento;
        END IF;

    ELSIF v_area_documento = 'Preinscripción' THEN
        SELECT EXISTS (
            SELECT 1 FROM "Requisitos"
            WHERE "claveTipoDocumento" = v_claveTipoDocumento 
              AND "etapa_proceso" = 'Preinscripción'
              AND "estado_requisito" = TRUE 
        ) INTO v_existe_en_requisitos_preinscripcion;

        SELECT EXISTS (
            SELECT 1 FROM "Requisitos"
            WHERE "claveTipoDocumento" = v_claveTipoDocumento 
              AND "etapa_proceso" = 'Inscripción'
              AND "estado_requisito" = TRUE 
        ) INTO v_existe_en_requisitos_inscripcion;

        IF NOT v_existe_en_requisitos_preinscripcion THEN
            RAISE EXCEPTION 'Error de consistencia: El documento "%" no figura en los requisitos de origen de Preinscripción.', 
                v_nombre_documento;
        END IF;

        IF NOT v_existe_en_requisitos_inscripcion THEN
            RAISE EXCEPTION 'Error de validación: El documento "%" es de Preinscripción y NO está autorizado en el catálogo de Inscripción.', 
                v_nombre_documento;
        END IF;

    ELSE
        RAISE EXCEPTION 'Error de área: Los documentos con área nativa "%" no tienen acceso al proceso de Inscripción.', 
            v_area_documento;
    END IF;

    -- 7. CANDADO DE CIERRE POR IGUALDAD EXACTA
    SELECT COUNT(*) INTO v_limite_requisitos
    FROM "Requisitos"
    WHERE "etapa_proceso" = 'Inscripción'
      AND "estado_requisito" = TRUE; 

    SELECT COUNT(*) INTO v_tuplas_actuales
    FROM "DetalleAdjuncionOriginal"
    WHERE "claveAdjOriginal" = p_claveAdjOriginal;

    IF v_tuplas_actuales >= v_limite_requisitos THEN
        RAISE EXCEPTION 'Error de proceso: Registro denegado. La adjunción ya cuenta con exactamente % documentos, cumpliendo con la igualdad de los % requisitos de Inscripción exigidos. El expediente está cerrado.', 
            v_tuplas_actuales, v_limite_requisitos;
    END IF;

    -- 8. VALIDACIÓN DE DUPLICIDAD DE DOCUMENTO (Candado Inter-Entrega)
    SELECT "claveAdjOriginal" INTO v_doc_ya_usado
    FROM "DetalleAdjuncionOriginal"
    WHERE "claveDocAspirante" = p_claveDocAspirante;

    IF v_doc_ya_usado IS NOT NULL THEN
        RAISE EXCEPTION 'Error de duplicidad: El documento % ya fue registrado previamente en la adjunción %.', 
            p_claveDocAspirante, v_doc_ya_usado;
    END IF;

    -- ============================================================
    -- 9. INSERCION DIRECTA (3 campos mapeados según el estado real de tu tabla)
    -- ============================================================
    INSERT INTO "DetalleAdjuncionOriginal" (
        "claveAdjOriginal",
        "claveDocAspirante",
        "ruta_pdf_original"
    )
    VALUES (
        p_claveAdjOriginal,
        p_claveDocAspirante,
        v_ruta_automatica -- <-- Insertamos de forma transparente la ruta recuperada
    );

    RAISE NOTICE 'Éxito: Documento "%" insertado. Estado del expediente: (% de % requisitos cubiertos). Ruta: %', 
        v_nombre_documento, (v_tuplas_actuales + 1), v_limite_requisitos, v_ruta_automatica;

END;
$$ LANGUAGE plpgsql;
