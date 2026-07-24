CREATE OR REPLACE PROCEDURE sp_insertar_usuario(
    p_nombre_usuario VARCHAR(50),
    p_contrasenia VARCHAR(255),
    p_correo_institucional VARCHAR(100),
    p_claveEmpleado VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_rol_nombre VARCHAR(20);
    v_estatus_laboral VARCHAR(20);
    v_total_requisitos_laborales INT;
    v_total_documentos_empleado INT;
    v_claveExpediente_emp VARCHAR(18);
    v_claveUsuario_generada VARCHAR(18);
    -- Expresión regular para validar formato de correo
    v_regex_correo TEXT := '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$';
BEGIN
    -- 1. VALIDACIÓN DE CAMPOS BÁSICOS Y UNICIDAD DE NOMBRE
    IF TRIM(p_nombre_usuario) = '' OR p_nombre_usuario IS NULL THEN
        RAISE EXCEPTION 'El nombre de usuario es obligatorio.';
    END IF;

    -- NUEVA VALIDACIÓN: Asegurar que la contraseña no sea nula ni vacía
    IF p_contrasenia IS NULL OR TRIM(p_contrasenia) = '' THEN
        RAISE EXCEPTION 'La contraseña es obligatoria y no puede estar vacía.';
    END IF;

    -- REGLA ESTRICTA: Unicidad absoluta del alias (Se ignora el campo estado)
    IF EXISTS (SELECT 1 FROM "Usuarios" WHERE nombre_usuario = TRIM(p_nombre_usuario)) THEN
        RAISE EXCEPTION 'Error: El nombre de usuario "%" ya está en uso.', p_nombre_usuario;
    END IF;

    -- ==========================================
    -- NUEVA VALIDACIÓN: FORMATO Y UNICIDAD DE CORREO
    -- ==========================================
    IF p_correo_institucional !~ v_regex_correo THEN
        RAISE EXCEPTION 'Error: El formato del correo institucional "%" no es válido.', p_correo_institucional;
    END IF;

    -- CANDADO: La validación local de correo ahora solo bloquea si pertenece a un usuario ACTIVO
    IF EXISTS (SELECT 1 FROM "Usuarios" WHERE correo_institucional = TRIM(p_correo_institucional) AND estado = TRUE) THEN
        RAISE EXCEPTION 'Error: El correo "%" ya está registrado con otro usuario.', p_correo_institucional;
    END IF;

    -- CANDADO TRANSVERSAL: Bloqueo si el correo ya existe en Receptores y está ACTIVO
    IF EXISTS (
        SELECT 1 
        FROM "Receptores" 
        WHERE "correo_destino" = LOWER(TRIM(p_correo_institucional)) 
          AND "estado" = TRUE
    ) THEN
        RAISE EXCEPTION 'Bloqueo de Seguridad: El correo "%" ya está registrado en el sistema bajo otro rol activo (Tutor o TutorAspirante).', p_correo_institucional;
    END IF;

    -- 2. VIAJE DE INFORMACIÓN (Empleado -> Rol Activo)
    -- Se mantiene intacto según tu requerimiento original
    SELECT 
        r.nombre_rol, 
        e.estatus_laboral, 
        e."claveExpediente"
    INTO 
        v_rol_nombre, 
        v_estatus_laboral, 
        v_claveExpediente_emp
    FROM "Empleados" e
    JOIN "EmpleadoRol" er ON e."claveEmpleado" = er."claveEmpleado"
    JOIN "Roles" r ON er."claveRol" = r."claveRol"
    WHERE e."claveEmpleado" = p_claveEmpleado 
      AND er.fecha_fin IS NULL; -- <--- CAMBIO: Solo permite roles activos

    -- A) Validar existencia y puesto activo
    IF v_rol_nombre IS NULL THEN
        RAISE EXCEPTION 'Bloqueo: El empleado % no tiene un puesto ACTIVO asignado en EmpleadoRol.', p_claveEmpleado;
    END IF;

    -- B) Bloqueo estricto a Intendentes
    IF v_rol_nombre = 'Intendente' THEN
        RAISE EXCEPTION 'Acceso Denegado: Los Intendentes no tienen permitido el acceso al sistema.';
    END IF;

    -- C) Validar estatus activo
    IF v_estatus_laboral <> 'Activo' THEN
        RAISE EXCEPTION 'Error: El empleado debe estar en estatus Activo.';
    END IF;

    -- ==========================================
    -- 3. AUDITORÍA DOCUMENTAL
    -- ==========================================
    -- CANDADO: Se añade "AND estado = TRUE" para contar solo los tipos de documentos vigentes
    SELECT COUNT(*) INTO v_total_requisitos_laborales 
    FROM "TipoDocumentos" 
    WHERE area = 'Laboral'
      AND estado = TRUE;

    SELECT COUNT(DISTINCT "claveTipoDocumento") INTO v_total_documentos_empleado
    FROM "Documentos"
    WHERE "claveExpediente" = v_claveExpediente_emp;

    IF v_total_documentos_empleado < v_total_requisitos_laborales THEN
        RAISE EXCEPTION 'Bloqueo: El Empleado no tiene la documentación laboral completa (% de %). Su ingreso al sistema es imposible.', 
                        v_total_documentos_empleado, v_total_requisitos_laborales;
    END IF;

    -- ==========================================
    -- 4. INSERCIÓN DE USUARIO Y CAPTURA DE ID
    -- ==========================================
    INSERT INTO "Usuarios" (
        nombre_usuario, 
        contrasenia, 
        correo_institucional, 
        "claveEmpleado"
    ) VALUES (
        TRIM(p_nombre_usuario), 
        p_contrasenia, 
        TRIM(p_correo_institucional), 
        p_claveEmpleado
    )
    RETURNING "claveUsuario" INTO v_claveUsuario_generada;

    -- ==========================================
    -- 5. ENCAPSULAMIENTO DEL RECEPTOR
    -- ==========================================
    CALL sp_generar_receptor(
        'Usuario', 
        v_claveUsuario_generada, 
        TRIM(p_correo_institucional)
    );

    RAISE NOTICE 'Éxito: Usuario "%" creado y habilitado como Receptor institucional.', p_nombre_usuario;

EXCEPTION
    WHEN unique_violation THEN
        RAISE EXCEPTION 'Error de integridad: El empleado ya cuenta con un usuario asignado o el correo está duplicado.';
    WHEN OTHERS THEN
        RAISE EXCEPTION '%', SQLERRM;
END;
$$;