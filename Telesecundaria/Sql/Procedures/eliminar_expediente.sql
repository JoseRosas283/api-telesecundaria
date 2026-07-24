CREATE OR REPLACE PROCEDURE eliminar_expediente(
    p_claveExpediente VARCHAR(18)
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_tipo_titular VARCHAR(20);
    v_clave_empleado VARCHAR(18);
    v_clave_usuario VARCHAR(18) := NULL; 
    v_sesiones_activas INTEGER := 0;
BEGIN
    -- ==========================================================
    -- 1. IDENTIFICAR TITULAR DEL EXPEDIENTE
    -- ==========================================================
    SELECT tipo_titular INTO v_tipo_titular
    FROM "Expedientes"
    WHERE "claveExpediente" = p_claveExpediente;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Error: El expediente % no existe.', p_claveExpediente;
    END IF;

    -- ==========================================================
    -- 2. CASO EMPLEADO: VALIDACIÓN DE USUARIO Y SESIÓN (EL BUG)
    -- ==========================================================
    IF v_tipo_titular = 'Empleado' THEN
        
        -- Buscamos la clave del empleado
        SELECT "claveEmpleado" INTO v_clave_empleado 
        FROM "Empleados" WHERE "claveExpediente" = p_claveExpediente;

        -- Buscamos si tiene usuario vinculado
        SELECT "claveUsuario" INTO v_clave_usuario
        FROM "Usuarios"
        WHERE "claveEmpleado" = v_clave_empleado;

        -- SI EXISTE USUARIO, CHECAMOS LOGUEOS
        IF v_clave_usuario IS NOT NULL THEN
            SELECT COUNT(*) INTO v_sesiones_activas
            FROM "Logueos" 
            WHERE "claveUsuario" = v_clave_usuario 
              AND estatus_intento = 'Exitoso'
              AND fecha_cierre IS NULL; 

            IF v_sesiones_activas > 0 THEN
                RAISE EXCEPTION 'Bloqueo de Seguridad: El usuario % tiene una sesión activa. Debe cerrar sesión para poder dar de baja el expediente.', v_clave_usuario;
            END IF;
        END IF;
    END IF;

    -- ==========================================================
    -- 3. EJECUCIÓN DE LA BAJA EN CASCADA
    -- ==========================================================
    
    -- A) Desactivar el Expediente siempre
    UPDATE "Expedientes" SET estado = FALSE WHERE "claveExpediente" = p_claveExpediente;

    -- B) Desactivación específica
    IF v_tipo_titular = 'Empleado' THEN
        -- Baja laboral
        UPDATE "Empleados" SET estatus_laboral = 'Baja' WHERE "claveEmpleado" = v_clave_empleado;

        -- Si tenía usuario, apagamos cuenta y RECEPTOR (con tus nombres de columna reales)
        IF v_clave_usuario IS NOT NULL THEN
            UPDATE "Usuarios" SET estado = FALSE WHERE "claveUsuario" = v_clave_usuario;
            
            -- Corregido: claveUsuario y tipo_receptor
            UPDATE "Receptores" SET estado = FALSE 
            WHERE "claveUsuario" = v_clave_usuario 
              AND tipo_receptor = 'Usuario'; 
        END IF;

        -- ==========================================================
        -- CAMBIO AÑADIDO: BAJA DEL ROL ACTIVO EN ENPLEADO_ROL
        -- ==========================================================
        CALL sp_dar_baja_rol_empleado(v_clave_empleado);

    ELSIF v_tipo_titular = 'Alumno' THEN
        UPDATE "Alumnos" SET estado = FALSE WHERE "claveExpediente" = p_claveExpediente;
    END IF;

    RAISE NOTICE 'Éxito: Baja integral procesada para el expediente %.', p_claveExpediente;

EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Fallo en la operación de baja: %', SQLERRM;
END;
$$;