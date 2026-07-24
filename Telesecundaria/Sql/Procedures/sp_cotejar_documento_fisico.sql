CREATE OR REPLACE PROCEDURE sp_cotejar_documento_fisico(
    p_claveEntrega VARCHAR(18),
    p_claveDocAspirante VARCHAR(18)
)
AS $$
DECLARE
    v_existe_entrega BOOLEAN;
    v_existe_en_digital BOOLEAN;
    v_entrega_duplicada VARCHAR(18);
BEGIN
    -- ============================================================
    -- 1. VERIFICACIÓN CRÍTICA: ¿EXISTE LA ENTREGA PADRE?
    -- ============================================================
    SELECT EXISTS (
        SELECT 1 FROM "Entregas" WHERE "claveEntrega" = p_claveEntrega
    ) INTO v_existe_entrega;

    IF NOT v_existe_entrega THEN
        RAISE EXCEPTION 'Error de existencia: La entrega % no se encuentra registrada en el sistema.', p_claveEntrega;
    END IF;

    -- ============================================================
    -- 2. EL VIAJE: VERIFICAR QUE EL DOCUMENTO EXISTA EN EL EXPEDIENTE DIGITAL
    -- ============================================================
    SELECT EXISTS (
        SELECT 1 
        FROM "AdjuncionesOriginales" ao
        INNER JOIN "DetalleAdjuncionOriginal" dao ON ao."claveAdjOriginal" = dao."claveAdjOriginal"
        WHERE ao."claveEntrega" = p_claveEntrega 
          AND dao."claveDocAspirante" = p_claveDocAspirante
    ) INTO v_existe_en_digital;

    IF NOT v_existe_en_digital THEN
        RAISE EXCEPTION 'Error de flujo: No se puede registrar la validación física. El documento % no ha sido cargado digitalmente en los PDFs de la entrega %.', 
            p_claveDocAspirante, p_claveEntrega;
    END IF;

    -- ============================================================
    -- 3. CANDADO INTER-ENTREGAS (EVITAR PLAGIO/DUPLICIDAD MULTI-ALUMNO)
    -- ============================================================
    SELECT "claveEntrega" INTO v_entrega_duplicada
    FROM "ValidacionDocumentos"
    WHERE "claveDocAspirante" = p_claveDocAspirante 
      AND "claveEntrega" <> p_claveEntrega
    LIMIT 1;

    IF v_entrega_duplicada IS NOT NULL THEN
        RAISE EXCEPTION 'Bloqueo de seguridad: El documento % NO se puede validar. Ya fue presentado y registrado físicamente en la entrega anterior %.', 
            p_claveDocAspirante, v_entrega_duplicada;
    END IF;

    -- ============================================================
    -- 4. REGISTRO DE COTEJO (ESTRICTO COMO 'Original')
    -- ============================================================
    INSERT INTO "ValidacionDocumentos" (
        "claveEntrega",
        "claveDocAspirante",
        "estatus_cotejo",
        "fecha_validacion"
    )
    VALUES (
        p_claveEntrega,
        p_claveDocAspirante,
        'Original', -- Estado directo y unificado para el bulto
        CURRENT_TIMESTAMP
    );

END;
$$ LANGUAGE plpgsql;
