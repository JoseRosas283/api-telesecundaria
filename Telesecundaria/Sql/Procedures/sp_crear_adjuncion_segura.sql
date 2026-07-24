CREATE OR REPLACE PROCEDURE sp_crear_adjuncion_segura(
    p_claveTutorAspirante VARCHAR(18),
    p_claveAspirante VARCHAR(18),
	OUT p_claveAdjuncion VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_tutor_real_aspirante VARCHAR(18);
    v_estatus_actual_aspirante VARCHAR(50);
    v_tiene_aspirantes_activos BOOLEAN;
    v_token_valido BOOLEAN;
    v_convocatoria_aspirante VARCHAR(18);
    v_convocatoria_activa BOOLEAN;
BEGIN
    -- 1. VALIDACIÓN DE EXISTENCIA DEL TUTOR
    IF NOT EXISTS (SELECT 1 FROM "TutorAspirante" WHERE "claveTutorAspirante" = p_claveTutorAspirante) THEN
        RAISE EXCEPTION 'Acceso Denegado: El Tutor con clave % no existe.', p_claveTutorAspirante;
    END IF;

    -- ==========================================================
    -- NUEVA VALIDACIÓN: TOKEN DE SESIÓN ACTIVO
    -- ==========================================================
    SELECT EXISTS (
        SELECT 1 FROM "TokenConvocatorias" 
        WHERE "claveTutorAspirante" = p_claveTutorAspirante 
          AND estado_sesion = TRUE 
          AND fecha_expiracion > CURRENT_TIMESTAMP
    ) INTO v_token_valido;

    IF NOT v_token_valido THEN
        RAISE EXCEPTION 'Acceso Denegado: Sesión inválida o expirada. Por favor, vuelva a ingresar al sistema.';
    END IF;

    -- ==========================================
    -- 2. VALIDACIÓN DE DISPONIBILIDAD GLOBAL DEL TUTOR
    -- ==========================================
    SELECT EXISTS (
        SELECT 1 FROM "Aspirantes" 
        WHERE "claveTutorAspirante" = p_claveTutorAspirante 
        AND estatus_aspirante <> 'Aceptado'
    ) INTO v_tiene_aspirantes_activos;

    IF NOT v_tiene_aspirantes_activos THEN
        RAISE EXCEPTION 'Operación cancelada: El tutor no tiene aspirantes relacionados o todos sus aspirantes ya han sido Aceptados.';
    END IF;

    -- 3. OBTENCIÓN DE DATOS DEL ASPIRANTE ESPECÍFICO
    SELECT "claveTutorAspirante", estatus_aspirante, "claveConvocatoria" 
    INTO v_tutor_real_aspirante, v_estatus_actual_aspirante, v_convocatoria_aspirante
    FROM "Aspirantes" 
    WHERE "claveAspirante" = p_claveAspirante;

    -- 4. VALIDACIÓN DE EXISTENCIA DEL ASPIRANTE
    IF v_tutor_real_aspirante IS NULL THEN
        RAISE EXCEPTION 'Error: El Aspirante con clave % no existe en el sistema.', p_claveAspirante;
    END IF;

    -- ==========================================================
    -- NUEVA VALIDACIÓN: BLINDAJE POR ESTADO DE CONVOCATORIA
    -- ==========================================================
    SELECT (estado = 'Publicada' AND activacion = TRUE) 
    INTO v_convocatoria_activa
    FROM "Convocatorias" 
    WHERE "claveConvocatoria" = v_convocatoria_aspirante;

    IF v_convocatoria_activa IS NOT TRUE THEN
        RAISE EXCEPTION 'Operación denegada: La convocatoria relacionada ya no se encuentra activa para recibir adjunciones.';
    END IF;

    -- 5. REGLA DE NEGOCIO: VALIDACIÓN DE ESTATUS INDIVIDUAL
    IF v_estatus_actual_aspirante = 'Aceptado' THEN
        RAISE EXCEPTION 'Operación denegada: Este aspirante específico ya tiene estatus ACEPTADO.';
    END IF;

    -- 6. VALIDACIÓN DE PERTENENCIA (CANDADO DE SEGURIDAD)
    IF v_tutor_real_aspirante <> p_claveTutorAspirante THEN
        RAISE EXCEPTION 'Violación de Seguridad: El aspirante % no está vinculado al tutor %.', 
                        p_claveAspirante, p_claveTutorAspirante;
    END IF;

    -- ==========================================================
    -- NUEVA VALIDACIÓN: TAPÓN DE UNICIDAD (EVITA COLAPSO)
    -- ==========================================================
    IF EXISTS (
        SELECT 1 FROM "Adjunciones" 
        WHERE "claveAspirante" = p_claveAspirante 
          AND "estatus_gral" = 'Pendiente'
    ) THEN
        RAISE EXCEPTION 'Operación cancelada: Ya existe una adjunción "Pendiente" para este aspirante. Finalice la actual antes de abrir otra.';
    END IF;

    -- 7. INSERCIÓN ATÓMICA (NACE ABIERTA)
    INSERT INTO "Adjunciones" (
        estatus_gral,
        estatus_operativo,
        "claveTutorAspirante",
        "claveAspirante"
    ) VALUES (
        'Pendiente', 
        'Abierta',
        p_claveTutorAspirante,
        p_claveAspirante
    );

	SELECT "claveAdjuncion" INTO p_claveAdjuncion
    FROM "Adjunciones"
    WHERE "claveAspirante" = p_claveAspirante
      AND estatus_operativo = 'Abierta'
    ORDER BY "claveAdjuncion" DESC
    LIMIT 1;

    RAISE NOTICE 'Validación exitosa: Adjunción Abierta creada para el aspirante %.', p_claveAspirante;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Operación cancelada: %', SQLERRM;
END;
$$;