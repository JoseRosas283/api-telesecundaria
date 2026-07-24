CREATE OR REPLACE PROCEDURE sp_asignar_alumno_grupo(
    p_claveAlumno   VARCHAR(18),
    p_claveGrupo    VARCHAR(18),
    p_claveUsuario  VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    -- Variables de Control
    v_claveExpediente    VARCHAR(18);
    v_claveCicloActivo   VARCHAR(18);
    v_usuario_activo     BOOLEAN;
    v_sesion_valida      BOOLEAN;
    v_rol_autorizado     BOOLEAN;
    v_tiene_permiso_mod  BOOLEAN;
BEGIN
    -- ============================================================
    -- 1. VALIDACIÓN DE EXISTENCIA FÍSICA (MATERIA PRIMA)
    -- Si el Alumno o el Grupo no existen, el proceso muere aquí.
    -- ============================================================
    
    -- Validar Alumno
    SELECT "claveExpediente" INTO v_claveExpediente 
    FROM "Alumnos" WHERE "claveAlumno" = p_claveAlumno;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: El alumno con clave "%" no existe.', p_claveAlumno;
    END IF;

    -- Validar Grupo (Tu observación: Si no hay grupo, no hay nada que hacer)
    IF NOT EXISTS (SELECT 1 FROM "Grupos" WHERE "claveGrupo" = p_claveGrupo) THEN
        RAISE EXCEPTION 'Error: El grupo destino "%" no existe en el catálogo.', p_claveGrupo;
    END IF;

    -- ============================================================
    -- 2. VALIDACIÓN DE REQUISITOS DEL ALUMNO (FILTROS DE PASO)
    -- ============================================================
    
    -- 2.1 Documentos en el Expediente
    IF NOT EXISTS (SELECT 1 FROM "Documentos" WHERE "claveExpediente" = v_claveExpediente) THEN
        RAISE EXCEPTION 'Bloqueo: El alumno no tiene documentos cargados. Expediente incompleto.';
    END IF;

    -- 2.2 Ciclo Escolar Activo
    SELECT "claveCiclo" INTO v_claveCicloActivo FROM "CiclosEscolares" WHERE estatus = TRUE;
    
    IF v_claveCicloActivo IS NULL THEN
        RAISE EXCEPTION 'Bloqueo: No hay un Ciclo Escolar activo en el sistema.';
    END IF;

    -- 2.3 Inscripción Administrativa (Lo caído, caído)
    IF NOT EXISTS (
        SELECT 1 FROM "Inscripciones" 
        WHERE "claveAlumno" = p_claveAlumno 
          AND "claveCiclo" = v_claveCicloActivo 
          AND estatus_inscripcion = 'INSCRITO'
    ) THEN
        RAISE EXCEPTION 'Bloqueo: El alumno no aparece como "INSCRITO" en este ciclo escolar.';
    END IF;

    -- ============================================================
    -- 3. VALIDACIÓN DE SEGURIDAD (EL OPERADOR)
    -- Una vez validado el negocio, verificamos quién aprieta el botón.
    -- ============================================================
    
    -- 3.1 Estado del Usuario
    SELECT estado INTO v_usuario_activo FROM "Usuarios" WHERE "claveUsuario" = p_claveUsuario;

    IF v_usuario_activo IS NULL THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario operador "%" no existe.', p_claveUsuario;
    ELSIF NOT v_usuario_activo THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario operador se encuentra inactivo.';
    END IF;

    -- 3.2 Sesión Abierta (Logueos)
    SELECT EXISTS (
        SELECT 1 FROM "Logueos" 
        WHERE "claveUsuario" = p_claveUsuario AND estatus_intento = 'Exitoso' AND fecha_cierre IS NULL
    ) INTO v_sesion_valida;

    IF NOT v_sesion_valida THEN
        RAISE EXCEPTION 'Acceso denegado: El usuario no cuenta con una sesión activa en el sistema.';
    END IF;

    -- 3.3 Jerarquía de Rol
    SELECT EXISTS (
        SELECT 1 FROM "Usuarios" u
        INNER JOIN "EmpleadoRol" er ON u."claveEmpleado" = er."claveEmpleado"
        INNER JOIN "Roles" r ON er."claveRol" = r."claveRol"
        WHERE u."claveUsuario" = p_claveUsuario 
          AND r.nombre_rol IN ('Administrativo', 'Directivo')
          AND er.fecha_fin IS NULL
    ) INTO v_rol_autorizado;

    IF NOT v_rol_autorizado THEN
        RAISE EXCEPTION 'Acceso denegado: Su rol no tiene privilegios para realizar asignaciones.';
    END IF;

    -- 3.4 Permiso Específico de Módulo
    SELECT EXISTS (
        SELECT 1 FROM "Usuarios" u
        INNER JOIN "EmpleadoRol" er ON u."claveEmpleado" = er."claveEmpleado"
        INNER JOIN "Permisos" p ON er."claveRol" = p."claveRol"
        INNER JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
        WHERE u."claveUsuario" = p_claveUsuario 
          AND m.nombre_modulo = 'AsignacionGrupo'
          AND p.puede_crear = TRUE 
          AND er.fecha_fin IS NULL
    ) INTO v_tiene_permiso_mod;

    IF NOT v_tiene_permiso_mod THEN
        RAISE EXCEPTION 'Acceso denegado: No cuenta con el permiso de creación en el módulo Asignación de Grupos.';
    END IF;

    -- ============================================================
    -- 4. OPERACIÓN Y SINCRONIZACIÓN
    -- ============================================================
    
    -- 4.1 Evitar doble asignación activa
    IF EXISTS (SELECT 1 FROM "AsignacionGrupo" 
               WHERE "claveAlumno" = p_claveAlumno AND "claveCiclo" = v_claveCicloActivo AND estatus = 'ACTIVO') THEN
        RAISE EXCEPTION 'Aviso: El alumno ya tiene un grupo asignado en el ciclo actual.';
    END IF;

    -- 4.2 Ejecutar Asignación
    INSERT INTO "AsignacionGrupo" ("claveAlumno", "claveGrupo", "claveUsuario", "claveCiclo", estatus)
    VALUES (p_claveAlumno, p_claveGrupo, p_claveUsuario, v_claveCicloActivo, 'ACTIVO');

    -- 4.3 Sincronizar tabla de Inscripciones
    UPDATE "Inscripciones" SET "claveGrupo" = p_claveGrupo 
    WHERE "claveAlumno" = p_claveAlumno AND "claveCiclo" = v_clave_ciclo_activo;

    RAISE NOTICE 'Éxito: Alumno vinculado al grupo % por el usuario %.', p_claveGrupo, p_claveUsuario;

END;
$$;
