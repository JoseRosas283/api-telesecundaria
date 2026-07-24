CREATE OR REPLACE PROCEDURE procesar_revision(
    p_claveUsuario VARCHAR(18), 
    p_observacion TEXT DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_claveAspirante VARCHAR(18);
    v_claveAdjuncion VARCHAR(18);
    v_nombre_asp TEXT;
    v_rol_clave VARCHAR(18);  -- Necesaria para buscar en Permisos
    v_rol_nombre VARCHAR(50);
    v_sesion_activa BOOLEAN;  -- Candado de Logueos
    v_puede_crear BOOLEAN;    -- Candado de Permisos
    v_nueva_pk_revision VARCHAR(18);
    r_doc RECORD; 
    v_usuario_existe BOOLEAN; 
BEGIN
    -- 0. VALIDACIÓN DE EXISTENCIA DEL USUARIO (El Escudo)
    SELECT EXISTS(SELECT 1 FROM "Usuarios" WHERE "claveUsuario" = p_claveUsuario) 
    INTO v_usuario_existe;

    IF NOT v_usuario_existe THEN
        RAISE EXCEPTION 'Error crítico: El usuario con clave % no existe en el sistema.', p_claveUsuario;
    END IF;

    -- 1. VALIDACIÓN DE SESIÓN ACTIVA (El Tercer Candado)
    -- Verifica que exista un logueo exitoso sin fecha de cierre para este usuario
    SELECT EXISTS (
        SELECT 1 FROM "Logueos" 
        WHERE "claveUsuario" = p_claveUsuario 
          AND estatus_intento = 'Exitoso' 
          AND fecha_cierre IS NULL
    ) INTO v_sesion_activa;

    IF NOT v_sesion_activa THEN
        RAISE EXCEPTION 'Acceso denegado: No se encontró una sesión activa (Logueo) para este usuario.';
    END IF;

    -- 2. VALIDACIÓN DE IDENTIDAD Y ROL
    SELECT r."claveRol", r.nombre_rol INTO v_rol_clave, v_rol_nombre
    FROM "Usuarios" u
    INNER JOIN "Empleados" e ON u."claveEmpleado" = e."claveEmpleado"
    INNER JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
    INNER JOIN "Roles" r ON er."claveRol" = r."claveRol" 
    WHERE u."claveUsuario" = p_claveUsuario 
      AND u.estado = TRUE 
      AND e.estatus_laboral = 'Activo';

    IF v_rol_nombre IS NULL OR v_rol_nombre NOT IN ('Directivo', 'Administrativo') THEN
        RAISE EXCEPTION 'Acceso denegado o permisos insuficientes.';
    END IF;

    -- 3. VALIDACIÓN DE PERMISOS DINÁMICOS (El Candado de Creación)
    -- Verifica si el rol tiene permiso de 'puede_crear' en el módulo de 'Revisiones'
    SELECT p.puede_crear INTO v_puede_crear
    FROM "Permisos" p
    INNER JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
    WHERE p."claveRol" = v_rol_clave 
      AND m.nombre_modulo = 'Revisiones'
      AND m.estado_modulo = TRUE;

    IF v_puede_crear IS NULL OR v_puede_crear = FALSE THEN
        RAISE EXCEPTION 'Acceso denegado: El rol % no tiene permiso de creación en el módulo de Revisiones.', v_rol_nombre;
    END IF;

    -- 4. EL BUSCADOR INTELIGENTE (Original)
    SELECT 
        f."claveAspirante", 
        a."claveAdjuncion",
        asp.nombre || ' ' || asp.apellido_paterno
    INTO 
        v_claveAspirante, 
        v_claveAdjuncion,
        v_nombre_asp
    FROM "FilaVirtual" f
    INNER JOIN "Aspirantes" asp ON f."claveAspirante" = asp."claveAspirante"
    INNER JOIN "Adjunciones" a ON a."claveAdjuncion" = (
        SELECT sub."claveAdjuncion" 
        FROM "Adjunciones" sub 
        WHERE sub."claveAspirante" = f."claveAspirante" 
          AND sub."estatus_operativo" = 'Cerrada' 
        ORDER BY sub."fecha_envio" DESC 
        LIMIT 1
    )
    WHERE NOT EXISTS (
        SELECT 1 FROM "Revisiones" r 
        WHERE r."claveAdjuncion" = a."claveAdjuncion"
    )
    ORDER BY f."numero_lugar" ASC
    LIMIT 1;

    -- 5. VALIDACIÓN DE EXISTENCIA EN FILA
    IF v_claveAdjuncion IS NULL THEN
        RAISE EXCEPTION 'No hay más aspirantes pendientes por revisar en la fila.';
    END IF;

    -- --- GENERACIÓN DE IDENTIDAD ---
    v_nueva_pk_revision := generar_clave_revision();

    -- 6. APERTURA DE CABECERA (La Cápsula)
    INSERT INTO "Revisiones" (
        "claveRevision",
        "claveAdjuncion", 
        "claveUsuario", 
        "estatus_revision", 
        "estado_operativo", 
        "observacion_general"
    ) VALUES (
        v_nueva_pk_revision,
        v_claveAdjuncion, 
        p_claveUsuario, 
        'Pendiente', 
        'Abierta', 
        COALESCE(p_observacion, 'Revisión automática iniciada por fila.')
    );

    -- 7. INICIALIZACIÓN DE DETALLES
    FOR r_doc IN (
        SELECT "claveDocAspirante" 
        FROM "DetalleAdjuncion" 
        WHERE "claveAdjuncion" = v_claveAdjuncion
    ) LOOP
        CALL registrar_detalle_revision(
            v_nueva_pk_revision, 
            r_doc."claveDocAspirante", 
            'Aceptado', 
            NULL
        );
    END LOOP;

    RAISE NOTICE 'Éxito: Sesión y permisos validados. Atendiendo a %. Revisión % inicializada.', v_nombre_asp, v_nueva_pk_revision;
END;
$$;