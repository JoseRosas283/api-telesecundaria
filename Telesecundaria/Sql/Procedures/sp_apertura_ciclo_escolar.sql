CREATE OR REPLACE PROCEDURE sp_apertura_ciclo_escolar(
    p_fecha_inicio DATE,
    p_fecha_fin    DATE
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_nombre_oficial VARCHAR(50);
    v_meses_duracion INTEGER;
    v_clave_nueva    VARCHAR(18); -- Para capturar la clave autogenerada
    v_fecha_cierre_inscripcion DATE; -- Para el cálculo de los 6 meses
BEGIN
    -- 1. IDIOMA PARA MESES EN ESPAÑOL
    SET lc_time = 'es_MX.UTF-8';

    -- 2. REGLA DE ORO: NO COEXISTENCIA
    IF EXISTS (SELECT 1 FROM "CiclosEscolares" WHERE estatus = TRUE) THEN
        RAISE EXCEPTION 'Bloqueo: No se puede abrir un nuevo ciclo porque ya existe uno ACTIVO. Debe cerrarlo primero.';
    END IF;

    -- 3. VALIDACIÓN DE CRONOLOGÍA
    IF p_fecha_fin <= p_fecha_inicio THEN
        RAISE EXCEPTION 'Error: La fecha de fin (%) debe ser posterior a la de inicio (%).', 
        p_fecha_fin, p_fecha_inicio;
    END IF;

    -- 4. VALIDACIÓN DE DURACIÓN (Máximo 12 meses)
    v_meses_duracion := (EXTRACT(YEAR FROM p_fecha_fin) - EXTRACT(YEAR FROM p_fecha_inicio)) * 12 +
                        (EXTRACT(MONTH FROM p_fecha_fin) - EXTRACT(MONTH FROM p_fecha_inicio));

    IF v_meses_duracion > 12 THEN
        RAISE EXCEPTION 'Bloqueo: La duración detectada es de % meses. El ciclo no puede durar más de un año.', v_meses_duracion;
    END IF;

    -- ============================================================================
    -- 4.1 CANDADO DE ANTICIPACIÓN (FUTURO)
    -- ============================================================================
    IF p_fecha_inicio > (CURRENT_DATE + INTERVAL '1 month') THEN
        RAISE EXCEPTION 'Bloqueo: No puedes aperturar con tanta anticipación. Máximo 30 días antes del inicio (%).', p_fecha_inicio;
    END IF;

    -- ============================================================================
    -- 4.2 CANDADO DE RETROACTIVIDAD ESTRICTA
    -- ============================================================================
    IF p_fecha_inicio < CURRENT_DATE THEN
        RAISE EXCEPTION 'Bloqueo: La fecha de inicio (%) no puede ser una fecha pasada. El ciclo debe iniciar hoy o en el futuro.', p_fecha_inicio;
    END IF;

    -- 5. NOMENCLATURA AUTOMÁTICA
    v_nombre_oficial := 'Ciclo Escolar ' || 
                        TRIM(to_char(p_fecha_inicio, 'TMMonth')) || ' ' || to_char(p_fecha_inicio, 'YYYY') || 
                        ' - ' || 
                        TRIM(to_char(p_fecha_fin, 'TMMonth')) || ' ' || to_char(p_fecha_fin, 'YYYY');

    -- 6. EVITAR DUPLICADOS HISTÓRICOS
    IF EXISTS (SELECT 1 FROM "CiclosEscolares" WHERE "nombreCiclo" = v_nombre_oficial) THEN
        RAISE EXCEPTION 'Error: El % ya existe en el historial.', v_nombre_oficial;
    END IF;

    -- 7. INSERCIÓN DEL CICLO Y OBTENCIÓN DE CLAVE
    -- Cambiado a CiclosEscolares (corrigiendo el typo 'CiclosEscenares')
    INSERT INTO "CiclosEscolares" (
        "nombreCiclo", 
        "fechaInicio", 
        "fechaFin", 
        estatus
    ) VALUES (
        v_nombre_oficial, 
        p_fecha_inicio, 
        p_fecha_fin, 
        TRUE 
    ) RETURNING "claveCiclo" INTO v_clave_nueva;

    -- ============================================================================
    -- 8. INICIALIZACIÓN INTERNA DEL PERIODO DE INSCRIPCIÓN (NUEVO)
    -- Lógica: Inicia hoy y cierra 6 meses antes de que termine el ciclo.
    -- ============================================================================
    v_fecha_cierre_inscripcion := p_fecha_fin - INTERVAL '6 months';

    INSERT INTO "Periodos" (
        "claveCiclo",
        nombre_periodo,
        fecha_inicio,
        fecha_fin,
        estado_periodo
    ) VALUES (
        v_clave_nueva,
        'Periodo de Inscripción ' || v_nombre_oficial,
        p_fecha_inicio,
        v_fecha_cierre_inscripcion,
        TRUE
    );

    RAISE NOTICE 'Éxito: Se ha aperturado el % correctamente.', v_nombre_oficial;
    RAISE NOTICE 'SISTEMA: El periodo de inscripción cerrará el %.', v_fecha_cierre_inscripcion;

END;
$$;