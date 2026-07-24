CREATE OR REPLACE PROCEDURE sp_inicializar_entrega(
    p_claveCita VARCHAR(18),
    p_claveUsuario VARCHAR(18)
)
LANGUAGE plpgsql -- Aseguramos el lenguaje en la cabecera
AS $$
DECLARE
    v_fecha_cita DATE;
    v_hora_cita TIME;
    v_tutor VARCHAR(18);
    v_estado_cita VARCHAR(20);
    v_es_docente BOOLEAN;
    v_existe_usuario BOOLEAN;
    
    -- Variables para la validación por niveles
    v_usuario_activo BOOLEAN;
    v_sesion_valida BOOLEAN;
    v_rol_autorizado BOOLEAN;
    v_tiene_permiso_modulo BOOLEAN;
    
    -- Variables para tu viaje relacional express
    v_claveRevision VARCHAR(18);
    v_claveAspirante VARCHAR(18);
BEGIN
    -- ============================================================
    -- 0. VALIDACIÓN DE EXISTENCIA Y ESTADO
    -- ============================================================
    SELECT estado INTO v_usuario_activo FROM "Usuarios" WHERE "claveUsuario" = p_claveUsuario;

    IF v_usuario_activo IS NULL THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario con la clave "%" no existe.', p_claveUsuario;
    ELSIF NOT v_usuario_activo THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario % se encuentra inactivo.', p_claveUsuario;
    END IF;

    -- ============================================================
    -- 0.1 VALIDACIÓN DE LOGUEO (SESIÓN)
    -- ============================================================
    SELECT EXISTS (
        SELECT 1 FROM "Logueos" 
        WHERE "claveUsuario" = p_claveUsuario 
          AND estatus_intento = 'Exitoso' 
          AND fecha_cierre IS NULL
    ) INTO v_sesion_valida;

    IF NOT v_sesion_valida THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario no tiene una sesión activa en Logueos.';
    END IF;

    -- ============================================================
    -- 1. VALIDACIÓN DE ROL (Jerarquía)
    -- ============================================================
    -- Solo Administrativos o Directivos pueden pasar este filtro
    SELECT EXISTS (
        SELECT 1 
        FROM "Usuarios" u
        INNER JOIN "EmpleadoRol" er ON u."claveEmpleado" = er."claveEmpleado"
        INNER JOIN "Roles" r ON er."claveRol" = r."claveRol"
        WHERE u."claveUsuario" = p_claveUsuario
          AND r.nombre_rol IN ('Administrativo', 'Directivo')
          AND er."fecha_fin" IS NULL
    ) INTO v_rol_autorizado;

    IF NOT v_rol_autorizado THEN
        RAISE EXCEPTION 'Acceso denegado: Su rol no está autorizado para gestionar entregas.';
    END IF;

    -- ============================================================
    -- 1.1 VALIDACIÓN DE PERMISOS (Matriz de Acceso)
    -- ============================================================
    -- Una vez validado el rol, verificamos que el permiso de 'crear' esté activo para 'Entregas'
    SELECT EXISTS (
        SELECT 1 
        FROM "Usuarios" u
        INNER JOIN "EmpleadoRol" er ON u."claveEmpleado" = er."claveEmpleado"
        INNER JOIN "Permisos" p ON er."claveRol" = p."claveRol"
        INNER JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
        WHERE u."claveUsuario" = p_claveUsuario
          AND m.nombre_modulo = 'Entregas'
          AND p.puede_crear = TRUE
          AND er."fecha_fin" IS NULL
    ) INTO v_tiene_permiso_modulo;

    IF NOT v_tiene_permiso_modulo THEN
        RAISE EXCEPTION 'Acceso denegado: No cuenta con el permiso específico de CREACIÓN en el módulo de Entregas.';
    END IF;

    -- ============================================================
    -- 2. EXTRACCIÓN Y VALIDACIÓN DE LA CITA
    -- ============================================================
    SELECT fecha_cita, hora_cita, "claveTutorAspirante", estado_cita, "claveRevision"
    INTO v_fecha_cita, v_hora_cita, v_tutor, v_estado_cita, v_claveRevision
    FROM "CitasInscripcion"
    WHERE "claveCita" = p_claveCita;

    -- Verificar que la cita exista
    IF v_tutor IS NULL THEN
        RAISE EXCEPTION 'Error: La cita especificada (%) no existe.', p_claveCita;
    END IF;

    -- Verificar que la cita esté 'Programada'
    IF v_estado_cita <> 'Programada' THEN
        RAISE EXCEPTION 'Error: Esta cita no se puede procesar porque su estado actual es "%".', v_estado_cita;
    END IF;

    -- ============================================================
    -- 3. EL VIAJE LOGÍSTICO INTERNO: Extraer al Aspirante
    -- ============================================================
    SELECT da."claveAspirante" 
    INTO v_claveAspirante
    FROM "DetalleRevision" dr
    INNER JOIN "DocumentosAspirante" da ON dr."claveDocAspirante" = da."claveDocAspirante"
    WHERE dr."claveRevision" = v_claveRevision
    LIMIT 1; 

    -- Seguridad: Validar que el viaje arrojó un alumno real
    IF v_claveAspirante IS NULL THEN
        RAISE EXCEPTION 'Error de consistencia: No se pudo localizar al Aspirante para la revisión %.', v_claveRevision;
    END IF;

    -- ============================================================
    -- 4. VALIDACIÓN DE TIEMPO (Día y Hora de la cita) - COMENTADO PARA PRUEBAS
    -- ============================================================
    -- Validar el Día
    -- IF CURRENT_DATE <> v_fecha_cita THEN
    --      RAISE EXCEPTION 'Error de vigencia: La cita está programada para el día %, y hoy es %.', 
    --          v_fecha_cita, CURRENT_DATE;
    -- END IF;

    -- Validar la Hora (No empezar antes de la hora citada)
    -- IF CURRENT_TIME < v_hora_cita THEN
    --      RAISE EXCEPTION 'Error de horario: No se puede iniciar la entrega aún. La cita está programada a las %, y la hora actual es %.', 
    --          v_hora_cita, CURRENT_TIME::TIME;
    -- END IF;

    -- ============================================================
    -- 5. INSERCIÓN DE LA ENTREGA Y MARCAJE DE ASISTENCIA
    -- ============================================================
    INSERT INTO "Entregas" (
        estado_final, 
        "claveCita", 
        "claveAspirante",      
        "claveTutorAspirante", 
        "claveUsuario"
    )
    VALUES (
        'pendiente', 
        p_claveCita, 
        v_claveAspirante, 
        v_tutor, 
        p_claveUsuario
    );

    -- CAMBIO LOGÍSTICO EXPLICITO: Pasamos la cita a estado 'Asistió'
    UPDATE "CitasInscripcion"
    SET estado_cita = 'Asistió'
    WHERE "claveCita" = p_claveCita;

    RAISE NOTICE 'Entrega inicializada con éxito. Usuario verificado por Logueo, Rol y Permisos.';

END;
$$;