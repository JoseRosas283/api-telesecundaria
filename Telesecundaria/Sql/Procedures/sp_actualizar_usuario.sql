CREATE OR REPLACE PROCEDURE sp_actualizar_usuario(
    p_claveUsuario VARCHAR(18),
    p_nuevo_nombre VARCHAR(50),
    p_nueva_contrasenia VARCHAR(255),
    p_nuevo_correo VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_nombre_actual VARCHAR(50);
    v_correo_actual VARCHAR(100);
    v_estatus_laboral VARCHAR(20); 
    v_claveReceptor_vinculada VARCHAR(18);
    v_nuevo_correo_limpio VARCHAR(100);
    v_regex_correo TEXT := '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$';
    
    -- Variable tipo RECORD para capturar la tupla actual completa de la consulta combinada
    v_usr_actual RECORD;
BEGIN
    -- =================================================================
    -- 1. VALIDACIÓN DE EXISTENCIA Y ESTADO LABORAL
    -- =================================================================
    -- Modificado para obtener todos los atributos (*) en el mismo viaje y alimentar el filtro ROW
    SELECT u.*, e.estatus_laboral
    INTO v_usr_actual
    FROM "Usuarios" u
    JOIN "Empleados" e ON u."claveEmpleado" = e."claveEmpleado"
    WHERE u."claveUsuario" = p_claveUsuario;

    IF v_usr_actual."claveUsuario" IS NULL THEN
        RAISE EXCEPTION 'Error: El usuario % no existe o no tiene un empleado vinculado.', p_claveUsuario;
    END IF;

    -- Mapeo de tus variables de control originales a partir del registro obtenido
    v_nombre_actual   := v_usr_actual.nombre_usuario;
    v_correo_actual   := v_usr_actual.correo_institucional;
    v_estatus_laboral := v_usr_actual.estatus_laboral;

    -- =========================================================================
    -- 1.5 FILTRO DE CAMBIOS REALES (AÑADIDO CON LA MISMA LÓGICA DE LOS ANTERIORES)
    -- =========================================================================
    -- Formateo previo rápido del correo para poder realizar una evaluación limpia y simétrica
    v_nuevo_correo_limpio := LOWER(TRIM(p_nuevo_correo));

    IF ROW(v_usr_actual.nombre_usuario, v_usr_actual.contrasenia, v_usr_actual.correo_institucional)
       IS NOT DISTINCT FROM
       ROW(TRIM(p_nuevo_nombre), p_nueva_contrasenia, v_nuevo_correo_limpio)
    THEN
        -- Indica éxito al cliente/API, deteniendo la ejecución sin gastar recursos en procesos posteriores
        RAISE NOTICE 'Éxito: Operación completada. Los datos enviados para el usuario % son idénticos a los actuales; no se realizaron cambios en la base de datos.', p_claveUsuario;
        RETURN; -- Detiene la ejecución limpia e inmediata antes de procesar el resto de la lógica
    END IF;

    -- 2. BLOQUEO POR BAJA LABORAL
    IF v_estatus_laboral = 'Baja' THEN
        RAISE EXCEPTION 'Bloqueo: No se puede actualizar el usuario %. El empleado vinculado tiene estatus de BAJA laboral.', p_claveUsuario;
    END IF;

    -- 3. VALIDACIÓN DE VACÍOS
    IF p_nuevo_nombre IS NULL OR TRIM(p_nuevo_nombre) = '' OR 
       p_nueva_contrasenia IS NULL OR p_nueva_contrasenia = '' OR 
       p_nuevo_correo IS NULL OR TRIM(p_nuevo_correo) = '' THEN
        RAISE EXCEPTION 'Error: Los campos nombre, contraseña y correo son obligatorios.';
    END IF;

    -- 4.1 VALIDACIÓN DE FORMATO REGEX
    IF v_nuevo_correo_limpio !~ v_regex_correo THEN
        RAISE EXCEPTION 'Error: El formato del correo "%" es incorrecto.', v_nuevo_correo_limpio;
    END IF;

    -- =================================================================
    -- 5. VALIDACIÓN DE UNICIDAD (SOLO SI HUBO CAMBIOS)
    -- =================================================================
    
    -- A) Unicidad del Nombre de Usuario
    IF TRIM(p_nuevo_nombre) <> v_nombre_actual THEN
        IF EXISTS (SELECT 1 FROM "Usuarios" WHERE nombre_usuario = TRIM(p_nuevo_nombre)) THEN
            RAISE EXCEPTION 'Error: El nombre de usuario "%" ya está en uso por otra cuenta.', p_nuevo_nombre;
        END IF;
    END IF;

    -- B) Unicidad del Correo (Local y Global)
    IF v_nuevo_correo_limpio <> v_correo_actual THEN
        
        -- ¿Existe ya en la tabla de Usuarios?
        IF EXISTS (
            SELECT 1 FROM "Usuarios" 
            WHERE correo_institucional = v_nuevo_correo_limpio
              AND estado = TRUE 
              AND "claveUsuario" <> p_claveUsuario 
        ) THEN
            RAISE EXCEPTION 'Error: El correo "%" ya está registrado en otra cuenta de usuario activa.', v_nuevo_correo_limpio;
        END IF;

        -- ¿Existe en la tabla global de Receptores (Activos) y no es el suyo propio?
        IF EXISTS (
            SELECT 1 FROM "Receptores" 
            WHERE correo_destino = v_nuevo_correo_limpio 
              AND estado = TRUE 
              AND ("claveUsuario" IS NULL OR "claveUsuario" <> p_claveUsuario)
        ) THEN
            RAISE EXCEPTION 'Bloqueo Global: El correo "%" ya está en uso por una cuenta activa (Tutor o Usuario) en el sistema.', v_nuevo_correo_limpio;
        END IF;
    END IF;

    -- =================================================================
    -- 6. ACTUALIZACIÓN DE TABLA USUARIOS
    -- =================================================================
    UPDATE "Usuarios" SET
        nombre_usuario = TRIM(p_nuevo_nombre),
        contrasenia = p_nueva_contrasenia,
        correo_institucional = v_nuevo_correo_limpio
    WHERE "claveUsuario" = p_claveUsuario;

    -- =================================================================
    -- 7. SINCRONIZACIÓN CON RECEPTORES
    -- =================================================================
    SELECT "claveReceptor" INTO v_claveReceptor_vinculada
    FROM "Receptores"
    WHERE "claveUsuario" = p_claveUsuario 
      AND tipo_receptor = 'Usuario';

    IF v_claveReceptor_vinculada IS NOT NULL THEN
        CALL sp_actualizar_receptor(
            v_claveReceptor_vinculada, 
            v_nuevo_correo_limpio
        );
    END IF;

    RAISE NOTICE 'Éxito: Datos de Usuario y Receptor para % actualizados correctamente.', p_claveUsuario;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Fallo en la actualización de usuario: %', SQLERRM;
END;
$$;