CREATE OR REPLACE PROCEDURE sp_registrar_carga_documental(
    p_clave_expediente VARCHAR(18),
    p_clave_usuario VARCHAR(18),
    OUT p_clave_carga_generada VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    -- Variables de Auditoría y Control
    v_usuario_existe        BOOLEAN;
    v_usuario_activo        BOOLEAN;
    v_sesion_activa         BOOLEAN;
    v_rol_clave             VARCHAR(18);
    v_rol_nombre            VARCHAR(50);
    v_puede_crear           BOOLEAN;
    
    -- Variables de Expediente
    v_tipo_titular          VARCHAR(20);
    v_clave_entrega         VARCHAR(18);
    v_existe_en_subtabla    BOOLEAN;
    v_msg_error             VARCHAR(250); -- Variable añadida para almacenar los mensajes específicos
    v_observaciones_auto    TEXT := 'Registro de carga documental para proceso de Alumnos o Empleados.';
BEGIN
    -- ============================================================
    -- 1. VALIDACIONES INICIALES DE ENTRADA
    -- ============================================================
    IF p_clave_expediente IS NULL OR TRIM(p_clave_expediente) = '' THEN
        RAISE EXCEPTION 'Error: La Clave de Expediente es obligatoria.';
    END IF;

    IF p_clave_usuario IS NULL OR TRIM(p_clave_usuario) = '' THEN
        RAISE EXCEPTION 'Error: La Clave de Usuario es obligatoria.';
    END IF;

    -- ============================================================
    -- 2. BLOQUE DE SEGURIDAD DE USUARIO (PASO A PASO)
    -- ============================================================
    
    -- A. Verificación de Existencia y Estado en Usuarios
    SELECT u.estado INTO v_usuario_activo
    FROM "Usuarios" u WHERE u."claveUsuario" = p_clave_usuario;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Acceso Denegado: El usuario con clave % no existe.', p_clave_usuario;
    END IF;

    IF v_usuario_activo = FALSE THEN
        RAISE EXCEPTION 'Acceso Denegado: El usuario se encuentra inactivo.';
    END IF;

    -- B. Verificación de Sesión (Logueos)
    SELECT EXISTS (
        SELECT 1 FROM "Logueos" 
        WHERE "claveUsuario" = p_clave_usuario 
          AND estatus_intento = 'Exitoso' 
          AND fecha_cierre IS NULL
    ) INTO v_sesion_activa;

    IF NOT v_sesion_activa THEN
        RAISE EXCEPTION 'Acceso Denegado: No se detectó una sesión activa para este usuario.';
    END IF;

    -- C. Obtención de Rol y Validación Laboral
    -- Validamos que el empleado esté activo y su asignación de rol sea vigente
    SELECT r."claveRol", r.nombre_rol 
    INTO v_rol_clave, v_rol_nombre
    FROM "Usuarios" u
    INNER JOIN "Empleados" e ON u."claveEmpleado" = e."claveEmpleado"
    INNER JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
    INNER JOIN "Roles" r ON er."claveRol" = r."claveRol"
    WHERE u."claveUsuario" = p_clave_usuario 
      AND e.estatus_laboral = 'Activo'
      AND CURRENT_DATE >= er.fecha_inicio 
      AND (er.fecha_fin IS NULL OR CURRENT_DATE <= er.fecha_fin);

    IF v_rol_clave IS NULL THEN
        RAISE EXCEPTION 'Acceso Denegado: El usuario no tiene un contrato laboral activo o un rol vigente.';
    END IF;

    -- D. Validación de Roles Autorizados (Filtro por nombre)
    IF v_rol_nombre NOT IN ('Administrativo', 'Directivo') THEN
        RAISE EXCEPTION 'Acceso Denegado: El rol % no tiene facultades para gestionar cargas.', v_rol_nombre;
    END IF;

    -- E. Candado de Permisos Dinámicos (Módulo Creacion)
    SELECT p.puede_crear INTO v_puede_crear
    FROM "Permisos" p
    INNER JOIN "Modulos" m ON p."claveModulo" = m."claveModulo"
    WHERE p."claveRol" = v_rol_clave 
      AND m.nombre_modulo = 'Creacion'
      AND m.estado_modulo = TRUE;

    IF v_puede_crear IS NOT TRUE THEN
        RAISE EXCEPTION 'Acceso Denegado: El usuario no cuenta con el permiso de creación en el módulo correspondiente.';
    END IF;

    -- ============================================================
    -- 3. VALIDACIÓN DEL EXPEDIENTE DESTINO
    -- ============================================================
    SELECT tipo_titular, "claveEntrega" 
    INTO v_tipo_titular, v_clave_entrega
    FROM "Expedientes" WHERE "claveExpediente" = p_clave_expediente;

    -- CAMBIO 1: Validación explícita de existencia del expediente base
    IF NOT FOUND OR v_tipo_titular IS NULL THEN
        RAISE EXCEPTION 'Error: El expediente % no existe en la base de datos.', p_clave_expediente;
    END IF;

    -- Regla de Negocio: Proceso Cerrado
    IF v_clave_entrega IS NOT NULL AND v_clave_entrega <> '' THEN
        RAISE EXCEPTION 'Bloqueo: El expediente ya cuenta con clave de entrega. No se permiten más cargas.';
    END IF;

    -- Inicializamos la variable de control de mensajes
    v_msg_error := NULL;

    -- Integridad con Sub-entidades y Mensajes específicos
    CASE v_tipo_titular
        WHEN 'Alumno' THEN
            SELECT EXISTS(SELECT 1 FROM "Alumnos" WHERE "claveExpediente" = p_clave_expediente) INTO v_existe_en_subtabla;
            IF NOT v_existe_en_subtabla THEN
                v_msg_error := 'Error de Integridad: El Alumno asociado a este expediente no existe en el sistema.';
            END IF;
            
        WHEN 'Empleado' THEN
            -- CAMBIO 2: Doble candado para Empleados (Existencia en Empleados + Rol vigente hoy)
            SELECT EXISTS(
                SELECT 1 
                FROM "Empleados" e
                INNER JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
                WHERE e."claveExpediente" = p_clave_expediente
                  AND CURRENT_DATE >= er.fecha_inicio 
                  AND (er.fecha_fin IS NULL OR CURRENT_DATE <= er.fecha_fin)
            ) INTO v_existe_en_subtabla;
            
            IF NOT v_existe_en_subtabla THEN
                v_msg_error := 'Error de Integridad: El Empleado asociado a este expediente no existe o no cuenta con un Rol activo asignado.';
            END IF;
            
        ELSE
            v_msg_error := 'Error de Integridad: No se encontró el registro de ' || v_tipo_titular || ' asociado a este expediente.';
    END CASE;

    -- Si se detectó algún fallo específico dentro del CASE, disparamos su excepción correspondiente
    IF v_msg_error IS NOT NULL THEN
        RAISE EXCEPTION '%', v_msg_error;
    END IF;

    -- ============================================================
    -- 4. INSERCIÓN FINAL
    -- ============================================================
    INSERT INTO "CargasDocumentos" (
        "claveExpediente", 
        "claveUsuario", 
        observaciones, 
        estatus_validacion
    ) VALUES (
        p_clave_expediente, 
        p_clave_usuario, 
        v_observaciones_auto, 
        'En Proceso'
    )
    RETURNING "claveCarga" INTO p_clave_carga_generada;

    RAISE NOTICE 'Éxito: Carga % generada por el usuario % (%)', p_clave_carga_generada, p_clave_usuario, v_rol_nombre;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION '%', SQLERRM;
END;
$$;