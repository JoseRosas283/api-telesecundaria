CREATE OR REPLACE PROCEDURE sp_realizar_inscripcion(
    p_claveAlumno    VARCHAR(18),
    p_nombre_usuario VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    -- Variables de Contexto
    v_clavePeriodo      VARCHAR(18);
    v_claveCiclo        VARCHAR(18);
    v_claveExpediente   VARCHAR(18);
    v_nombreCompleto    TEXT;

    -- Variables de Seguridad
    v_claveUsuario_op   VARCHAR(18);
    v_usuario_activo    BOOLEAN;
    v_sesion_valida     BOOLEAN;
    v_rol_autorizado    BOOLEAN;
    v_permiso_modulo    BOOLEAN;
BEGIN
    -- ============================================================
    -- 1. VALIDACIÓN DEL ALUMNO (MATERIA PRIMA)
    -- ============================================================
    SELECT a."claveExpediente", 
           CONCAT(e.nombre, ' ', e.apellido_paterno, ' ', e.apellido_materno)
    INTO v_claveExpediente, v_nombreCompleto
    FROM "Alumnos" a
    INNER JOIN "Expedientes" e ON a."claveExpediente" = e."claveExpediente"
    WHERE a."claveAlumno" = p_claveAlumno;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: El alumno "%" no existe.', p_claveAlumno;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM "Documentos" WHERE "claveExpediente" = v_claveExpediente) THEN
        RAISE EXCEPTION 'Bloqueo: El expediente de % no tiene documentos.', v_nombreCompleto;
    END IF;

    -- ============================================================
    -- 2. VALIDACIÓN DE TIEMPO (PERIODO ACTIVO)
    -- ============================================================
    SELECT "clavePeriodo", "claveCiclo" INTO v_clavePeriodo, v_claveCiclo
    FROM "Periodos"
    WHERE estado_periodo = TRUE 
      AND CURRENT_DATE BETWEEN fecha_inicio AND fecha_fin
    LIMIT 1;

    IF v_clavePeriodo IS NULL THEN
        RAISE EXCEPTION 'Bloqueo: No hay un Periodo de inscripción activo hoy.';
    END IF;

    -- ============================================================
    -- 3. SEGURIDAD DEL USUARIO (PASO A PASO)
    -- ============================================================
    
    -- 3.1 Identidad y Estado
    SELECT "claveUsuario", estado INTO v_claveUsuario_op, v_usuario_activo 
    FROM "Usuarios" WHERE "nombre_usuario" = TRIM(p_nombre_usuario);

    IF v_claveUsuario_op IS NULL THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario "%" no existe.', p_nombre_usuario;
    ELSIF NOT v_usuario_activo THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario "%" está inactivo.', p_nombre_usuario;
    END IF;

    -- 3.2 Sesión (Logueos)
    SELECT EXISTS (
        SELECT 1 FROM "Logueos" 
        WHERE "claveUsuario" = v_claveUsuario_op AND estatus_intento = 'Exitoso' AND fecha_cierre IS NULL
    ) INTO v_sesion_valida;

    IF NOT v_sesion_valida THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario % no tiene una sesión activa.', p_nombre_usuario;
    END IF;

    -- 3.3 VALIDACIÓN DE ROL (APARTE)
    -- ¿Tiene el cargo necesario?
    SELECT EXISTS (
        SELECT 1 FROM "EmpleadoRol" er
        INNER JOIN "Roles" r ON er."claveRol" = r."claveRol"
        INNER JOIN "Usuarios" u ON u."claveEmpleado" = er."claveEmpleado"
        WHERE u."claveUsuario" = v_claveUsuario_op
          AND r.nombre_rol IN ('Administrativo', 'Directivo')
          AND er.fecha_fin IS NULL
    ) INTO v_rol_autorizado;

    IF NOT v_rol_autorizado THEN
        RAISE EXCEPTION 'Acceso denegado: El rol de % no está autorizado para inscribir.', p_nombre_usuario;
    END IF;

    -- 3.4 VALIDACIÓN DE PERMISOS (APARTE)
    -- Teniendo el rol, ¿tiene el permiso 'crear' en este módulo?
    SELECT EXISTS (
        SELECT 1 FROM "Permisos" p
        INNER JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
        INNER JOIN "EmpleadoRol" er ON p."claveRol" = er."claveRol"
        INNER JOIN "Usuarios" u ON u."claveEmpleado" = er."claveEmpleado"
        WHERE u."claveUsuario" = v_claveUsuario_op
          AND m.nombre_modulo = 'Inscribir'
          AND p.puede_crear = TRUE
          AND er.fecha_fin IS NULL
    ) INTO v_permiso_modulo;

    IF NOT v_permiso_modulo THEN
        RAISE EXCEPTION 'Acceso denegado: % no tiene habilitado el permiso de creación en "Inscribir".', p_nombre_usuario;
    END IF;

    -- ============================================================
    -- 4. NEGOCIO E INSERCIÓN
    -- ============================================================
    IF EXISTS (SELECT 1 FROM "Inscripciones" WHERE "claveAlumno" = p_claveAlumno AND "clavePeriodo" = v_clavePeriodo) THEN
        RAISE EXCEPTION 'Error: % ya está inscrito en este periodo.', v_nombreCompleto;
    END IF;

    INSERT INTO "Inscripciones" (
        "claveAlumno", "claveCiclo", "clavePeriodo", "claveGrupo",
        "claveUsuario", "clavePago", estatus_inscripcion, observaciones
    ) VALUES (
        p_claveAlumno, v_claveCiclo, v_clavePeriodo, NULL,
        v_claveUsuario_op, NULL, 'INSCRITO',
        'Inscripción inicial de ' || v_nombreCompleto || '. Realizada por: ' || p_nombre_usuario
    );

    RAISE NOTICE 'Inscripción exitosa para %. (Grupo y Pago: NULL)', v_nombreCompleto;

END;
$$;
